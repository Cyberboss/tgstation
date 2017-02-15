var/datum/subsystem/job/SSjob

/datum/subsystem/job
	name = "Jobs"
	init_order = 5
	flags = SS_NO_FIRE

	var/list/occupations = list()			//List of all jobs
	var/list/name_occupations = list()		//Dict of all jobs, keys are titles
	var/list/type_occupations = list()		//Dict of all jobs, keys are types
	var/list/inductee_occupations = list() 	//List of all jobs chars unlock.
	var/list/unassigned = list()			//Players who need jobs
	var/list/job_debug = list()				//Debug info
	var/initial_players_to_assign = 0 		//used for checking against population caps

/datum/subsystem/job/New()
	NEW_SS_GLOBAL(SSjob)


/datum/subsystem/job/Initialize(timeofday)
	if(!occupations.len)
		SetupOccupations()
	if(config.load_jobs_from_txt)
		LoadJobs()
	..()


/datum/subsystem/job/proc/SetupOccupations(faction = "Station")
	occupations = list()
	var/list/all_jobs = subtypesof(/datum/job)
	if(!all_jobs.len)
		world << "<span class='boldannounce'>Error setting up jobs, no job datums found</span>"
		return 0

	for(var/J in all_jobs)
		var/datum/job/job = new J()
		if(!job)
			continue
		if(job.faction != faction)
			continue
		if(!job.config_check())
			continue
		if(job.rank_succession_level == INDUCTEE_SUCCESSION_LEVEL)
			inductee_occupations += job
			//setup base roles cause of bullshit ~
		occupations += job
		name_occupations[job.title] = job
		type_occupations[J] = job

	for(var/mob/new_player/p in mob_list)
		if(!p.client.prefs.roles || !p.client.prefs.roles.len)
			p.client.prefs.roles = GetInducteeJobsRanks()

	return 1


/datum/subsystem/job/proc/Debug(text)
	if(!Debug2)
		return 0
	job_debug.Add(text)
	return 1


/datum/subsystem/job/proc/GetJob(rank)
	if(!occupations.len)
		SetupOccupations()
	return name_occupations[rank]

/datum/subsystem/job/proc/GetJobType(jobtype)
	if(!occupations.len)
		SetupOccupations()
	return type_occupations[jobtype]

/datum/subsystem/job/proc/GetInducteeJobsRanks()
	var/list/iJobs = new/list()
	for(var/datum/job/j in inductee_occupations)
		iJobs += j.title


/datum/subsystem/job/proc/AssignRole(mob/new_player/player, rank, latejoin=0)
	//warning("Running AR, Player: [player], Rank: [rank], LJ: [latejoin]")
	Debug("Running AR, Player: [player], Rank: [rank], LJ: [latejoin]")
	if(player && player.mind && rank)
		var/datum/job/job = GetJob(rank)
		if(!job)
			return 0
		if(jobban_isbanned(player, rank))
			return 0
		if(!job.player_old_enough(player.client))
			return 0
		var/position_limit = job.total_positions
		if(!latejoin)
			position_limit = job.spawn_positions
		Debug("Player: [player] is now Rank: [rank], JCP:[job.current_positions], JPL:[position_limit]")
		player.mind.assigned_role = rank
		unassigned -= player
		job.current_positions++
		return 1
	//warning("AR has failed, Player: [player], Rank: [rank]")
	Debug("AR has failed, Player: [player], Rank: [rank]")
	return 0


/datum/subsystem/job/proc/FindOccupationCandidates(datum/job/job, level, flag)
	//warning("Running FOC, Job: [job], Level: [level], Flag: [flag]")
	Debug("Running FOC, Job: [job], Level: [level], Flag: [flag]")
	var/list/candidates = list()
	for(var/mob/new_player/player in unassigned)
		if(jobban_isbanned(player, job.title))
			Debug("FOC isbanned failed, Player: [player]")
			continue
		if(!job.player_old_enough(player.client))
			Debug("FOC player not old enough, Player: [player]")
			continue
		if(flag && (!(flag in player.client.prefs.be_special)))
			Debug("FOC flag failed, Player: [player], Flag: [flag], ")
			continue
		if(player.mind && job.title in player.mind.restricted_roles)
			Debug("FOC incompatible with antagonist role, Player: [player]")
			continue
		if(config.enforce_human_authority && !player.client.prefs.pref_species.qualifies_for_rank(job.title, player.client.prefs.features))
			Debug("FOC non-human failed, Player: [player]")
			continue
		if(player.client.prefs.GetJobDepartment(job, level))
			Debug("FOC pass, Player: [player], Level:[level]")
			candidates += player
	return candidates

/datum/subsystem/job/proc/GiveRandomJob(mob/new_player/player)
	Debug("GRJ Giving random job, Player: [player]")
	for(var/datum/job/job in shuffle(occupations))
		if(!job)
			continue

		if(istype(job, GetJob("Assistant"))) // We don't want to give him assistant, that's boring!
			continue

		if(job.title in command_positions) //If you want a command position, select it!
			continue

		if(jobban_isbanned(player, job.title))
			Debug("GRJ isbanned failed, Player: [player], Job: [job.title]")
			continue

		if(!job.player_old_enough(player.client))
			Debug("GRJ player not old enough, Player: [player]")
			continue

		if(player.mind && job.title in player.mind.restricted_roles)
			Debug("GRJ incompatible with antagonist role, Player: [player], Job: [job.title]")
			continue

		if(config.enforce_human_authority && !player.client.prefs.pref_species.qualifies_for_rank(job.title, player.client.prefs.features))
			Debug("GRJ non-human failed, Player: [player]")
			continue


		if((job.current_positions < job.spawn_positions) || job.spawn_positions == -1)
			Debug("GRJ Random job given, Player: [player], Job: [job]")
			AssignRole(player, job.title)
			unassigned -= player
			break

/datum/subsystem/job/proc/ResetOccupations()
	for(var/mob/new_player/player in player_list)
		if((player) && (player.mind))
			player.mind.assigned_role = null
			player.mind.special_role = null
	SetupOccupations()
	unassigned = list()
	return


//This proc is called before the level loop of DivideOccupations() and will try to select a head, ignoring ALL non-head preferences for every level until
//it locates a head or runs out of levels to check
//This is basically to ensure that there's atleast a few heads in the round
/datum/subsystem/job/proc/FillHeadPosition()
	for(var/level = 1 to 3)
		for(var/command_position in command_positions)
			var/datum/job/job = GetJob(command_position)
			if(!job)
				continue
			if((job.current_positions >= job.total_positions) && job.total_positions != -1)
				continue
			var/list/candidates = FindOccupationCandidates(job, level)
			if(!candidates.len)
				continue
			var/mob/new_player/candidate = pick(candidates)
			if(AssignRole(candidate, command_position))
				return 1
	return 0


//This proc is called at the start of the level loop of DivideOccupations() and will cause head jobs to be checked before any other jobs of the same level
//This is also to ensure we get as many heads as possible
/datum/subsystem/job/proc/CheckHeadPositions(level)
	for(var/command_position in command_positions)
		var/datum/job/job = GetJob(command_position)
		if(!job)
			continue
		if((job.current_positions >= job.total_positions) && job.total_positions != -1)
			continue
		var/list/candidates = FindOccupationCandidates(job, level)
		if(!candidates.len)
			continue
		var/mob/new_player/candidate = pick(candidates)
		AssignRole(candidate, command_position)
	return


/datum/subsystem/job/proc/FillAIPosition(unassigned)
	var/ai_selected = 0
	var/datum/job/job = GetJob("AI")
	if(!job)
		return 0
	for(var/i = job.total_positions, i > 0, i--)
		for(var/level = 1 to 3)
			var/list/candidates = list()
			candidates = FindOccupationCandidates(job, level)
			if(candidates.len)
				var/mob/new_player/candidate = pick(candidates)
				if(AssignRole(candidate, "AI"))
					ai_selected++
					break
	if(ai_selected)
		return 1
	return 0


/** Proc DivideOccupations
 *  fills var "assigned_role" for all ready players.
 *  This proc must not have any side effect besides of modifying "assigned_role".
 *	Totally redone for Artemis Station's role system ~rj
 **/
/datum/subsystem/job/proc/DivideOccupations()
	//Setup new player list and get the jobs list
	Debug("Running DO")

	//Holder for Triumvirate is stored in the ticker, this just processes it
	if(ticker)
		for(var/datum/job/ai/A in occupations)
			if(ticker.triai)
				A.spawn_positions = 3

	//Get the players who are ready
	for(var/mob/new_player/player in player_list)
		if(player.ready && player.mind && !player.mind.assigned_role)
			unassigned += player

	initial_players_to_assign = unassigned.len

	Debug("DO, Len: [unassigned.len]")
	if(unassigned.len == 0)
		return 0

	//Scale number of open security officer slots to population
	setup_officer_positions()

	//Jobs will have fewer access permissions if the number of players exceeds the threshold defined in game_options.txt
	if(config.minimal_access_threshold)
		if(config.minimal_access_threshold > unassigned.len)
			config.jobs_have_minimal_access = 0
		else
			config.jobs_have_minimal_access = 1

	//Shuffle players and jobs
	unassigned = shuffle(unassigned)

	HandleFeedbackGathering()

	//People who have assistant as high should just get that role
	Debug("DO, Running Assistant Check 1")
	var/tmp/assigned = 0
	for(var/mob/new_player/player in unassigned)
		if(player.client.prefs.roles["Assistant"] == "HIGH")
			Debug("AC1 pass, Player: [player]")
			AssignRole(player, "Assistant")
			unassigned -= player
			assigned ++
		if(assigned >= STARTING_ASSISTANTS)
			break

	Debug("DO, AC1 end")

	Debug("DO, Standart Check start")
	var/list/to_assign = unassigned.Copy()
	var/list/never_jobs = new/list()
	for(var/mob/new_player/player in to_assign)
		if(PopcapReached())
			RejectPlayer(player)
			Debug("DO, Standart Check start, POPCAP REACHED!")
			break

		var/datum/job/high = null
		var/list/med_jobs = new/list()
		var/list/low_jobs = new/list()

		for(var/role in player.client.prefs.roles)
			if(player.client.prefs.roles[role] == "HIGH")
				high = GetJob(role)
			if(player.client.prefs.roles[role] == "MEDIUM")
				med_jobs += GetJob(role)
			if(player.client.prefs.roles[role] == "LOW")
				low_jobs += GetJob(role)
			if(player.client.prefs.roles[role] == "NEVER")
				never_jobs += GetJob(role)

		//Assign first choise
		if(!isnull(high) && istype(high))
			if(high.spawn_positions >= high.current_positions || high.spawn_positions == -1)
				AssignRole(player, high.title)
				unassigned -= player
				continue

		var/succes = 0

		//Assign seccond choise
		if(!isnull(med_jobs) && med_jobs.len)
			for(var/datum/job/job in shuffle(med_jobs))
				if(job.spawn_positions >= job.current_positions || job.spawn_positions == -1)
					succes = 1
					AssignRole(player, job.title)
					unassigned -= player
					break
			if(succes)
				continue

		//Assign thirth choise
		if(!isnull(low_jobs) && low_jobs.len)
			for(var/datum/job/job in shuffle(low_jobs))
				if(job.spawn_positions >= job.current_positions || job.spawn_positions == -1)
					succes = 1
					AssignRole(player, job.title)
					unassigned -= player
					break
			if(succes)
				continue
	Debug("DO, Standard Check end")

	to_assign = unassigned.Copy()

	Debug("DO, Running AC2")

	// For those who wanted to be assistant if their preferences were filled, here you go.
	for(var/mob/new_player/player in to_assign)
		if(PopcapReached())
			RejectPlayer(player)
		if(player.client.prefs.joblessrole == BEASSISTANT)
			Debug("AC2 Assistant located, Player: [player]")
			AssignRole(player, "Assistant")
			unassigned -= player
		else // For those who don't want to play if their preference were filled, back you go.
			RejectPlayer(player)

	to_assign = unassigned.Copy()

	//Assign random job that the player has unlocked.
	for(var/mob/new_player/player in to_assign)
		if(!isnull(never_jobs) && never_jobs.len)
			var/succes = 0
			for(var/datum/job/job in shuffle(never_jobs))
				if(job.spawn_positions >= job.current_positions || job.spawn_positions == -1)
					succes = 1
					AssignRole(player, job.title)
					unassigned -= player
					break
			if(succes)
				continue

	//Finally players that really cannot join because all the jobs they can occcupie are taken.
	for(var/mob/new_player/player in unassigned)
		RejectPlayer(player)
	return 1

//Gives the player the stuff he should have with his rank
/datum/subsystem/job/proc/EquipRank(mob/living/H, rank, joined_late=0)
	var/datum/job/job = GetJob(rank)

	H.job = rank

	//If we joined at roundstart we should be positioned at our workstation
	if(!joined_late)
		var/obj/S = null
		for(var/obj/effect/landmark/start/sloc in start_landmarks_list)
			if(sloc.name != rank)
				S = sloc //so we can revert to spawning them on top of eachother if something goes wrong
				continue
			if(locate(/mob/living) in sloc.loc)
				continue
			S = sloc
			break
		if(!S) //if there isn't a spawnpoint send them to latejoin, if there's no latejoin go yell at your mapper
			world.log << "Couldn't find a round start spawn point for [rank]"
			S = pick(latejoin)
		if(!S) //final attempt, lets find some area in the arrivals shuttle to spawn them in to.
			world.log << "Couldn't find a round start latejoin spawn point."
			for(var/turf/T in get_area_turfs(/area/shuttle/arrival))
				if(!T.density)
					var/clear = 1
					for(var/obj/O in T)
						if(O.density)
							clear = 0
							break
					if(clear)
						S = T
						continue
		if(istype(S, /obj/effect/landmark) && isturf(S.loc))
			H.loc = S.loc

	if(H.mind)
		H.mind.assigned_role = rank

	if(job)
		var/new_mob = job.equip(H)
		if(ismob(new_mob))
			H = new_mob

	H << "<b>You are the [rank].</b>"
	H << "<b>As the [rank] you answer directly to [job.supervisors]. Special circumstances may change this.</b>"
	H << "<b>To speak on your departments radio, use the :h button. To see others, look closely at your headset.</b>"
	if(job.req_admin_notify)
		H << "<b>You are playing a job that is important for Game Progression. If you have to disconnect, please notify the admins via adminhelp.</b>"
	if(config.minimal_access_threshold)
		H << "<FONT color='blue'><B>As this station was initially staffed with a [config.jobs_have_minimal_access ? "full crew, only your job's necessities" : "skeleton crew, additional access may"] have been added to your ID card.</B></font>"

	if(job && H)
		job.after_spawn(H)

	return H


/datum/subsystem/job/proc/setup_officer_positions()
	var/datum/job/J = SSjob.GetJob("Security Officer")
	if(!J)
		throw EXCEPTION("setup_officer_positions(): Security officer job is missing")

	if(config.security_scaling_coeff > 0)
		if(J.spawn_positions > 0)
			var/officer_positions = min(12, max(J.spawn_positions, round(unassigned.len/config.security_scaling_coeff))) //Scale between configured minimum and 12 officers
			Debug("Setting open security officer positions to [officer_positions]")
			J.total_positions = officer_positions
			J.spawn_positions = officer_positions

	//Spawn some extra eqipment lockers if we have more than 5 officers
	var/equip_needed = J.total_positions
	if(equip_needed < 0) // -1: infinite available slots
		equip_needed = 12
	for(var/i=equip_needed-5, i>0, i--)
		if(secequipment.len)
			var/spawnloc = secequipment[1]
			new /obj/structure/closet/secure_closet/security/sec(spawnloc)
			secequipment -= spawnloc
		else //We ran out of spare locker spawns!
			break


/datum/subsystem/job/proc/LoadJobs()
	var/jobstext = return_file_text("config/jobs.txt")
	for(var/datum/job/J in occupations)
		var/regex/jobs = new("[J.title]=(-1|\\d+),(-1|\\d+)")
		jobs.Find(jobstext)
		J.total_positions = text2num(jobs.group[1])
		J.spawn_positions = text2num(jobs.group[2])

/datum/subsystem/job/proc/HandleFeedbackGathering()
	for(var/datum/job/job in occupations)
		var/tmp_str = "|[job.title]|"

		var/level1 = 0 //high
		var/level2 = 0 //medium
		var/level3 = 0 //low
		var/level4 = 0 //never
		var/level5 = 0 //banned
		var/level6 = 0 //account too young
		for(var/mob/new_player/player in player_list)
			if(!(player.ready && player.mind && !player.mind.assigned_role))
				continue //This player is not ready
			if(jobban_isbanned(player, job.title))
				level5++
				continue
			if(!job.player_old_enough(player.client))
				level6++
				continue
			if(player.client.prefs.GetJobDepartment(job, 1))
				level1++
			else if(player.client.prefs.GetJobDepartment(job, 2))
				level2++
			else if(player.client.prefs.GetJobDepartment(job, 3))
				level3++
			else level4++ //not selected

		tmp_str += "HIGH=[level1]|MEDIUM=[level2]|LOW=[level3]|NEVER=[level4]|BANNED=[level5]|YOUNG=[level6]|-"
		feedback_add_details("job_preferences",tmp_str)

/datum/subsystem/job/proc/PopcapReached()
	if(config.hard_popcap || config.extreme_popcap)
		var/relevent_cap = max(config.hard_popcap, config.extreme_popcap)
		if((initial_players_to_assign - unassigned.len) >= relevent_cap)
			return 1
	return 0

/datum/subsystem/job/proc/RejectPlayer(mob/new_player/player)
	if(player.mind && player.mind.special_role)
		return
	if(PopcapReached())
		Debug("Popcap overflow Check observer located, Player: [player]")
	player << "<b>You have failed to qualify for any job you desired.</b>"
	unassigned -= player
	player.ready = 0


/datum/subsystem/job/Recover()
	var/oldjobs = SSjob.occupations
	spawn(20)
		for (var/datum/job/J in oldjobs)
			spawn(-1)
				var/datum/job/newjob = GetJob(J.title)
				if (!istype(newjob))
					return
				newjob.total_positions = J.total_positions
				newjob.spawn_positions = J.spawn_positions
				newjob.current_positions = J.current_positions