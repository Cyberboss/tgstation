/datum/preferences
	var/job_cargo_high = 0
	var/job_cargo_med = 0
	var/job_cargo_low = 0

	var/job_sci_high = 0
	var/job_sci_med = 0
	var/job_sci_low = 0

	var/job_med_high = 0
	var/job_med_med = 0
	var/job_med_low = 0

	var/job_sec_high = 0
	var/job_sec_med = 0
	var/job_sec_low = 0

	var/job_eng_high = 0
	var/job_eng_med = 0
	var/job_eng_low = 0

	var/job_external_high = 0
	var/job_external_med = 0
	var/job_external_low = 0

/datum/preferences/process_link(mob/user, list/href_list)
	if(href_list["jobbancheck"])
		var/job = sanitizeSQL(href_list["jobbancheck"])
		var/sql_ckey = sanitizeSQL(user.ckey)
		var/DBQuery/query_get_jobban = dbcon.NewQuery("SELECT reason, bantime, duration, expiration_time, a_ckey FROM [format_table_name("ban")] WHERE ckey = '[sql_ckey]' AND job = '[job]' AND (bantype = 'JOB_PERMABAN'  OR (bantype = 'JOB_TEMPBAN' AND expiration_time > Now())) AND isnull(unbanned)")
		if(!query_get_jobban.Execute())
			var/err = query_get_jobban.ErrorMsg()
			log_game("SQL ERROR obtaining reason from ban table. Error : \[[err]\]\n")
			return
		if(query_get_jobban.NextRow())
			var/reason = query_get_jobban.item[1]
			var/bantime = query_get_jobban.item[2]
			var/duration = query_get_jobban.item[3]
			var/expiration_time = query_get_jobban.item[4]
			var/a_ckey = query_get_jobban.item[5]
			var/text
			text = "<span class='redtext'>You, or another user of this computer, ([user.ckey]) is banned from playing [job]. The ban reason is:<br>[reason]<br>This ban was applied by [a_ckey] on [bantime]"
			if(text2num(duration) > 0)
				text += ". The ban is for [duration] minutes and expires on [expiration_time] (server time)"
			text += ".</span>"
			user << text
		return

	if(href_list["preference"] == "job")
		switch(href_list["task"])
			if("close")
				user << browse(null, "window=mob_occupation")
				ShowChoices(user)
			if("reset")
				ResetJobs()
				SetChoices(user)
			if("random")
				switch(joblessrole)
					if(RETURNTOLOBBY)
						if(jobban_isbanned(user, "Assistant"))
							joblessrole = BERANDOMJOB
						else
							joblessrole = BEASSISTANT
					if(BEASSISTANT)
						joblessrole = BERANDOMJOB
					if(BERANDOMJOB)
						joblessrole = RETURNTOLOBBY
				SetChoices(user)
			if("setJobLevel")
				UpdateJobPreference(user, href_list["text"], text2num(href_list["level"]))
			else
				SetChoices(user)
		return 1

	switch(href_list["task"])
		if("random")
			switch(href_list["preference"])
				if("name")
					real_name = pref_species.random_name(gender,1)
				if("age")
					age = rand(AGE_MIN, AGE_MAX)
				if("hair")
					hair_color = random_short_color()
				if("hair_style")
					hair_style = random_hair_style(gender)
				if("facial")
					facial_hair_color = random_short_color()
				if("facial_hair_style")
					facial_hair_style = random_facial_hair_style(gender)
				if("underwear")
					underwear = random_underwear(gender)
				if("undershirt")
					undershirt = random_undershirt(gender)
				if("socks")
					socks = random_socks()
				if("eyes")
					eye_color = random_eye_color()
				if("s_tone")
					skin_tone = random_skin_tone()
				if("bag")
					backbag = pick(backbaglist)
				if("all")
					random_character()

		if("input")
			switch(href_list["preference"])
				if("ghostform")
					if(unlock_content)
						var/new_form = input(user, "Thanks for supporting BYOND - Choose your ghostly form:","Thanks for supporting BYOND",null) as null|anything in ghost_forms
						if(new_form)
							ghost_form = new_form
				if("ghostorbit")
					if(unlock_content)
						var/new_orbit = input(user, "Thanks for supporting BYOND - Choose your ghostly orbit:","Thanks for supporting BYOND", null) as null|anything in ghost_orbits
						if(new_orbit)
							ghost_orbit = new_orbit

				if("ghostaccs")
					var/new_ghost_accs = alert("Do you want your ghost to show full accessories where possible, hide accessories but still use the directional sprites where possible, or also ignore the directions and stick to the default sprites?",,GHOST_ACCS_FULL_NAME, GHOST_ACCS_DIR_NAME, GHOST_ACCS_NONE_NAME)
					switch(new_ghost_accs)
						if(GHOST_ACCS_FULL_NAME)
							ghost_accs = GHOST_ACCS_FULL
						if(GHOST_ACCS_DIR_NAME)
							ghost_accs = GHOST_ACCS_DIR
						if(GHOST_ACCS_NONE_NAME)
							ghost_accs = GHOST_ACCS_NONE

				if("ghostothers")
					var/new_ghost_others = alert("Do you want the ghosts of others to show up as their own setting, as their default sprites or always as the default white ghost?",,GHOST_OTHERS_THEIR_SETTING_NAME, GHOST_OTHERS_DEFAULT_SPRITE_NAME, GHOST_OTHERS_SIMPLE_NAME)
					switch(new_ghost_others)
						if(GHOST_OTHERS_THEIR_SETTING_NAME)
							ghost_others = GHOST_OTHERS_THEIR_SETTING
						if(GHOST_OTHERS_DEFAULT_SPRITE_NAME)
							ghost_others = GHOST_OTHERS_DEFAULT_SPRITE
						if(GHOST_OTHERS_SIMPLE_NAME)
							ghost_others = GHOST_OTHERS_SIMPLE

				if("name")
					var/new_name
					if(pref_species.id == "vatgrown")
						new_name = reject_bad_name( input(user, "Choose your character's name:", "Character Preference") as text|null, 1)
					else
						new_name = reject_bad_name( input(user, "Choose your character's name:", "Character Preference")  as text|null )
					if(new_name)
						real_name = new_name
					else
						user << "<font color='red'>Invalid name. Your name should be at least 2 and at most [MAX_NAME_LEN] characters long. It may only contain the characters A-Z, a-z, -, ' and .</font>"

				if("age")
					var/new_age = input(user, "Choose your character's age:\n([AGE_MIN]-[AGE_MAX])", "Character Preference") as num|null
					if(new_age)
						age = max(min( round(text2num(new_age)), AGE_MAX),AGE_MIN)

				if("metadata")
					var/new_metadata = input(user, "Enter any information you'd like others to see, such as Roleplay-preferences:", "Game Preference" , metadata)  as message|null
					if(new_metadata)
						metadata = sanitize(copytext(new_metadata,1,MAX_MESSAGE_LEN))

				if("hair")
					var/new_hair = input(user, "Choose your character's hair colour:", "Character Preference") as null|color
					if(new_hair)
						hair_color = sanitize_hexcolor(new_hair)


				if("hair_style")
					var/new_hair_style
					if(gender == MALE)
						new_hair_style = input(user, "Choose your character's hair style:", "Character Preference")  as null|anything in hair_styles_male_list
					else
						new_hair_style = input(user, "Choose your character's hair style:", "Character Preference")  as null|anything in hair_styles_female_list
					if(new_hair_style)
						hair_style = new_hair_style

				if("next_hair_style")
					if (gender == MALE)
						hair_style = next_list_item(hair_style, hair_styles_male_list)
					else
						hair_style = next_list_item(hair_style, hair_styles_female_list)

				if("previous_hair_style")
					if (gender == MALE)
						hair_style = previous_list_item(hair_style, hair_styles_male_list)
					else
						hair_style = previous_list_item(hair_style, hair_styles_female_list)

				if("facial")
					var/new_facial = input(user, "Choose your character's facial-hair colour:", "Character Preference") as null|color
					if(new_facial)
						facial_hair_color = sanitize_hexcolor(new_facial)

				if("facial_hair_style")
					var/new_facial_hair_style
					if(gender == MALE)
						new_facial_hair_style = input(user, "Choose your character's facial-hair style:", "Character Preference")  as null|anything in facial_hair_styles_male_list
					else
						new_facial_hair_style = input(user, "Choose your character's facial-hair style:", "Character Preference")  as null|anything in facial_hair_styles_female_list
					if(new_facial_hair_style)
						facial_hair_style = new_facial_hair_style

				if("next_facehair_style")
					if (gender == MALE)
						facial_hair_style = next_list_item(facial_hair_style, facial_hair_styles_male_list)
					else
						facial_hair_style = next_list_item(facial_hair_style, facial_hair_styles_female_list)

				if("previous_facehair_style")
					if (gender == MALE)
						facial_hair_style = previous_list_item(facial_hair_style, facial_hair_styles_male_list)
					else
						facial_hair_style = previous_list_item(facial_hair_style, facial_hair_styles_female_list)

				if("underwear")
					var/new_underwear
					if(gender == MALE)
						new_underwear = input(user, "Choose your character's underwear:", "Character Preference")  as null|anything in underwear_m
					else
						new_underwear = input(user, "Choose your character's underwear:", "Character Preference")  as null|anything in underwear_f
					if(new_underwear)
						underwear = new_underwear

				if("undershirt")
					var/new_undershirt
					if(gender == MALE)
						new_undershirt = input(user, "Choose your character's undershirt:", "Character Preference") as null|anything in undershirt_m
					else
						new_undershirt = input(user, "Choose your character's undershirt:", "Character Preference") as null|anything in undershirt_f
					if(new_undershirt)
						undershirt = new_undershirt

				if("socks")
					var/new_socks
					new_socks = input(user, "Choose your character's socks:", "Character Preference") as null|anything in socks_list
					if(new_socks)
						socks = new_socks

				if("eyes")
					var/new_eyes = input(user, "Choose your character's eye colour:", "Character Preference") as color|null
					if(new_eyes)
						eye_color = sanitize_hexcolor(new_eyes)

				if("species")

					var/result = input(user, "Select a species", "Species Selection") as null|anything in roundstart_species

					if(result)
						var/newtype = roundstart_species[result]
						pref_species = new newtype()
						//Now that we changed our species, we must verify that the mutant colour is still allowed.
						var/temp_hsv = RGBtoHSV(features["mcolor"])
						if(features["mcolor"] == "#000" || (!(MUTCOLORS_PARTSONLY in pref_species.species_traits) && ReadHSV(temp_hsv)[3] < ReadHSV("#7F7F7F")[3]))
							features["mcolor"] = pref_species.default_color
					if(pref_species.id != "vatgrown")
						real_name = reject_bad_name(real_name)

				if("mutant_color")
					var/new_mutantcolor = input(user, "Choose your character's alien/mutant color:", "Character Preference") as color|null
					if(new_mutantcolor)
						var/temp_hsv = RGBtoHSV(new_mutantcolor)
						if(new_mutantcolor == "#000000")
							features["mcolor"] = pref_species.default_color
						else if((MUTCOLORS_PARTSONLY in pref_species.species_traits) || ReadHSV(temp_hsv)[3] >= ReadHSV("#7F7F7F")[3]) // mutantcolors must be bright, but only if they affect the skin
							features["mcolor"] = sanitize_hexcolor(new_mutantcolor)
						else
							user << "<span class='danger'>Invalid color. Your color is not bright enough.</span>"

				if("tail_lizard")
					var/new_tail
					new_tail = input(user, "Choose your character's tail:", "Character Preference") as null|anything in tails_list_lizard
					if(new_tail)
						features["tail_lizard"] = new_tail

				if("tail_human")
					var/new_tail
					new_tail = input(user, "Choose your character's tail:", "Character Preference") as null|anything in tails_list_human
					if(new_tail)
						features["tail_human"] = new_tail

				if("snout")
					var/new_snout
					new_snout = input(user, "Choose your character's snout:", "Character Preference") as null|anything in snouts_list
					if(new_snout)
						features["snout"] = new_snout

				if("horns")
					var/new_horns
					new_horns = input(user, "Choose your character's horns:", "Character Preference") as null|anything in horns_list
					if(new_horns)
						features["horns"] = new_horns

				if("tentacles")
					var/new_tentacles
					new_tentacles = input(user, "Choose your character's tentacles:", "Character Preference") as anything in tentacles_list
					if(new_tentacles)
						features["tentacles"] = new_tentacles

				if("ears")
					var/new_ears
					new_ears = input(user, "Choose your character's ears:", "Character Preference") as null|anything in ears_list
					if(new_ears)
						features["ears"] = new_ears

				if("wings")
					var/new_wings
					new_wings = input(user, "Choose your character's wings:", "Character Preference") as null|anything in r_wings_list
					if(new_wings)
						features["wings"] = new_wings

				if("frills")
					var/new_frills
					new_frills = input(user, "Choose your character's frills:", "Character Preference") as null|anything in frills_list
					if(new_frills)
						features["frills"] = new_frills

				if("spines")
					var/new_spines
					new_spines = input(user, "Choose your character's spines:", "Character Preference") as null|anything in spines_list
					if(new_spines)
						features["spines"] = new_spines

				if("body_markings")
					var/new_body_markings
					new_body_markings = input(user, "Choose your character's body markings:", "Character Preference") as null|anything in body_markings_list
					if(new_body_markings)
						features["body_markings"] = new_body_markings

				if("legs")
					var/new_legs
					new_legs = input(user, "Choose your character's legs:", "Character Preference") as null|anything in legs_list
					if(new_legs)
						features["legs"] = new_legs

				if("s_tone")
					var/new_s_tone = input(user, "Choose your character's skin-tone:", "Character Preference")  as null|anything in skin_tones
					if(new_s_tone)
						skin_tone = new_s_tone

				if("ooccolor")
					var/new_ooccolor = input(user, "Choose your OOC colour:", "Game Preference") as color|null
					if(new_ooccolor)
						ooccolor = sanitize_ooccolor(new_ooccolor)

				if("bag")
					var/new_backbag = input(user, "Choose your character's style of bag:", "Character Preference")  as null|anything in backbaglist
					if(new_backbag)
						backbag = new_backbag

				if("uplink_loc")
					var/new_loc = input(user, "Choose your character's traitor uplink spawn location:", "Character Preference") as null|anything in uplink_spawn_loc_list
					if(new_loc)
						uplink_spawn_loc = new_loc

				if("clown_name")
					var/new_clown_name = reject_bad_name( input(user, "Choose your character's clown name:", "Character Preference")  as text|null )
					if(new_clown_name)
						custom_names["clown"] = new_clown_name
					else
						user << "<font color='red'>Invalid name. Your name should be at least 2 and at most [MAX_NAME_LEN] characters long. It may only contain the characters A-Z, a-z, -, ' and .</font>"

				if("mime_name")
					var/new_mime_name = reject_bad_name( input(user, "Choose your character's mime name:", "Character Preference")  as text|null )
					if(new_mime_name)
						custom_names["mime"] = new_mime_name
					else
						user << "<font color='red'>Invalid name. Your name should be at least 2 and at most [MAX_NAME_LEN] characters long. It may only contain the characters A-Z, a-z, -, ' and .</font>"

				if("ai_name")
					var/new_ai_name = reject_bad_name( input(user, "Choose your character's AI name:", "Character Preference")  as text|null, 1 )
					if(new_ai_name)
						custom_names["ai"] = new_ai_name
					else
						user << "<font color='red'>Invalid name. Your name should be at least 2 and at most [MAX_NAME_LEN] characters long. It may only contain the characters A-Z, a-z, 0-9, -, ' and .</font>"

				if("cyborg_name")
					var/new_cyborg_name = reject_bad_name( input(user, "Choose your character's cyborg name:", "Character Preference")  as text|null, 1 )
					if(new_cyborg_name)
						custom_names["cyborg"] = new_cyborg_name
					else
						user << "<font color='red'>Invalid name. Your name should be at least 2 and at most [MAX_NAME_LEN] characters long. It may only contain the characters A-Z, a-z, 0-9, -, ' and .</font>"

				if("religion_name")
					var/new_religion_name = reject_bad_name( input(user, "Choose your character's religion:", "Character Preference")  as text|null )
					if(new_religion_name)
						custom_names["religion"] = new_religion_name
					else
						user << "<font color='red'>Invalid name. Your name should be at least 2 and at most [MAX_NAME_LEN] characters long. It may only contain the characters A-Z, a-z, -, ' and .</font>"

				if("deity_name")
					var/new_deity_name = reject_bad_name( input(user, "Choose your character's deity:", "Character Preference")  as text|null )
					if(new_deity_name)
						custom_names["deity"] = new_deity_name
					else
						user << "<font color='red'>Invalid name. Your name should be at least 2 and at most [MAX_NAME_LEN] characters long. It may only contain the characters A-Z, a-z, -, ' and .</font>"

				if("sec_dept")
					var/department = input(user, "Choose your prefered security department:", "Security Departments") as null|anything in security_depts_prefs
					if(department)
						prefered_security_department = department

				if ("preferred_map")
					var/maplist = list()
					var/default = "Default"
					if (config.defaultmap)
						default += " ([config.defaultmap.friendlyname])"
					for (var/M in config.maplist)
						var/datum/votablemap/VM = config.maplist[M]
						var/friendlyname = "[VM.friendlyname] "
						if (VM.voteweight <= 0)
							friendlyname += " (disabled)"
						maplist[friendlyname] = VM.name
					maplist[default] = null
					var/pickedmap = input(user, "Choose your preferred map. This will be used to help weight random map selection.", "Character Preference")  as null|anything in maplist
					if (pickedmap)
						preferred_map = maplist[pickedmap]

				if ("clientfps")
					var/version_message
					if (user.client && user.client.byond_version < 511)
						version_message = "\nYou need to be using byond version 511 or later to take advantage of this feature, your version of [user.client.byond_version] is too low"
					if (world.byond_version < 511)
						version_message += "\nThis server does not currently support client side fps. You can set now for when it does."
					var/desiredfps = input(user, "Choose your desired fps.[version_message]\n(0 = synced with server tick rate (currently:[world.fps]))", "Character Preference", clientfps)  as null|num
					if (!isnull(desiredfps))
						clientfps = desiredfps
						if (world.byond_version >= 511 && user.client && user.client.byond_version >= 511)
							user.client.vars["fps"] = clientfps
				if("ui")
					var/pickedui = input(user, "Choose your UI style.", "Character Preference")  as null|anything in list("Midnight", "Plasmafire", "Retro", "Slimecore", "Operative", "Clockwork")
					if(pickedui)
						UI_style = pickedui

		else
			switch(href_list["preference"])
				if("publicity")
					if(unlock_content)
						toggles ^= MEMBER_PUBLIC
				if("gender")
					if(gender == MALE)
						gender = FEMALE
					else
						gender = MALE
					underwear = random_underwear(gender)
					undershirt = random_undershirt(gender)
					socks = random_socks()
					facial_hair_style = random_facial_hair_style(gender)
					hair_style = random_hair_style(gender)

				if("hotkeys")
					hotkeys = !hotkeys

				if("tgui_fancy")
					tgui_fancy = !tgui_fancy
				if("tgui_lock")
					tgui_lock = !tgui_lock

				if("hear_adminhelps")
					toggles ^= SOUND_ADMINHELP
				if("announce_login")
					toggles ^= ANNOUNCE_LOGIN

				if("be_special")
					var/be_special_type = href_list["be_special_type"]
					if(be_special_type in be_special)
						be_special -= be_special_type
					else
						be_special += be_special_type

				if("name")
					be_random_name = !be_random_name

				if("all")
					be_random_body = !be_random_body

				if("hear_midis")
					toggles ^= SOUND_MIDI

				if("lobby_music")
					toggles ^= SOUND_LOBBY
					if(toggles & SOUND_LOBBY)
						user << sound(ticker.login_music, repeat = 0, wait = 0, volume = 85, channel = 1)
					else
						user.stopLobbySound()

				if("ghost_ears")
					chat_toggles ^= CHAT_GHOSTEARS

				if("ghost_sight")
					chat_toggles ^= CHAT_GHOSTSIGHT

				if("ghost_whispers")
					chat_toggles ^= CHAT_GHOSTWHISPER

				if("ghost_radio")
					chat_toggles ^= CHAT_GHOSTRADIO

				if("ghost_pda")
					chat_toggles ^= CHAT_GHOSTPDA

				if("pull_requests")
					chat_toggles ^= CHAT_PULLR

				if("allow_midround_antag")
					toggles ^= MIDROUND_ANTAG

				if("parallaxup")
					parallax = Wrap(parallax + 1, PARALLAX_INSANE, PARALLAX_DISABLE + 1)
					if (parent && parent.mob && parent.mob.hud_used)
						parent.mob.hud_used.update_parallax_pref()

				if("parallaxdown")
					parallax = Wrap(parallax - 1, PARALLAX_INSANE, PARALLAX_DISABLE + 1)
					if (parent && parent.mob && parent.mob.hud_used)
						parent.mob.hud_used.update_parallax_pref()

				if("save")
					save_preferences()
					save_character()

				if("load")
					load_preferences()
					load_character()

				if("changeslot")
					if(!load_character(text2num(href_list["num"])))
						random_character()
						real_name = random_unique_name(gender)
						save_character()

				if("tab")
					if (href_list["tab"])
						current_tab = text2num(href_list["tab"])

	ShowChoices(user)
	return 1


/datum/preferences/SetJobPreferenceLevel(datum/job/job, level)
	if (!job)
		return 0

	if (level == 1) // to high
		// remove any other job(s) set to high
		job_civilian_med |= job_civilian_high
		job_eng_med |= job_eng_high
		job_sec_med |= job_sec_high
		job_med_med |= job_med_high
		job_sci_med |= job_sci_high
		job_cargo_med |= job_cargo_high
		job_external_med |= job_external_high
		job_civilian_high = 0
		job_sec_high = 0
		job_eng_high = 0
		job_sci_high = 0
		job_med_high = 0
		job_external_high = 0
		job_cargo_high = 0

	if (job.department_flag == CIVILIAN)
		job_civilian_low &= ~job.flag
		job_civilian_med &= ~job.flag
		job_civilian_high &= ~job.flag

		switch(level)
			if (1)
				job_civilian_high |= job.flag
			if (2)
				job_civilian_med |= job.flag
			if (3)
				job_civilian_low |= job.flag

		return 1
	else if (job.department_flag == SEC)
		job_sec_low &= ~job.flag
		job_sec_med &= ~job.flag
		job_sec_high &= ~job.flag

		switch(level)
			if (1)
				job_sec_high |= job.flag
			if (2)
				job_sec_med |= job.flag
			if (3)
				job_sec_low |= job.flag

		return 1
	else if (job.department_flag == ENG)
		job_eng_low &= ~job.flag
		job_eng_med &= ~job.flag
		job_eng_high &= ~job.flag

		switch(level)
			if (1)
				job_eng_high |= job.flag
			if (2)
				job_eng_med |= job.flag
			if (3)
				job_eng_low |= job.flag

		return 1
	else if (job.department_flag == SCI)
		job_sci_low &= ~job.flag
		job_sci_med &= ~job.flag
		job_sci_high &= ~job.flag

		switch(level)
			if (1)
				job_sci_high |= job.flag
			if (2)
				job_sci_med |= job.flag
			if (3)
				job_sci_low |= job.flag

		return 1
	else if (job.department_flag == MED)
		job_med_low &= ~job.flag
		job_med_med &= ~job.flag
		job_med_high &= ~job.flag

		switch(level)
			if (1)
				job_med_high |= job.flag
			if (2)
				job_med_med |= job.flag
			if (3)
				job_med_low |= job.flag

		return 1
	else if (job.department_flag == CARGO)
		job_cargo_low &= ~job.flag
		job_cargo_med &= ~job.flag
		job_cargo_high &= ~job.flag

		switch(level)
			if (1)
				job_cargo_high |= job.flag
			if (2)
				job_cargo_med |= job.flag
			if (3)
				job_cargo_low |= job.flag

		return 1
	else if (job.department_flag == SOLGOV)
		job_external_low &= ~job.flag
		job_external_med &= ~job.flag
		job_external_high &= ~job.flag

		switch(level)
			if (1)
				job_external_high |= job.flag
			if (2)
				job_external_med |= job.flag
			if (3)
				job_external_low |= job.flag

		return 1

	return 0

/datum/preferences/UpdateJobPreference(mob/user, role, desiredLvl)
	if(!SSjob || SSjob.occupations.len <= 0)
		return
	var/datum/job/job = SSjob.GetJob(role)

	if(!job)
		user << browse(null, "window=mob_occupation")
		ShowChoices(user)
		return

	if (!isnum(desiredLvl))
		user << "<span class='danger'>UpdateJobPreference - desired level was not a number. Please notify coders!</span>"
		ShowChoices(user)
		return

	if(role == "Assistant")
		if(job_civilian_low & job.flag)
			job_civilian_low &= ~job.flag
		else
			job_civilian_low |= job.flag
		SetChoices(user)
		return 1

	SetJobPreferenceLevel(job, desiredLvl)
	SetChoices(user)

	return 1


/datum/preferences/ResetJobs()

	job_civilian_high = 0
	job_civilian_med = 0
	job_civilian_low = 0

	job_med_high = 0
	job_med_med = 0
	job_med_low = 0

	job_sci_high = 0
	job_sci_med = 0
	job_sci_low = 0

	job_sec_high = 0
	job_sec_med = 0
	job_sec_low = 0

	job_eng_high = 0
	job_eng_med = 0
	job_eng_low = 0

	job_cargo_high = 0
	job_cargo_med = 0
	job_cargo_low = 0

	job_external_high = 0
	job_external_med = 0
	job_external_low = 0


/datum/preferences/GetJobDepartment(datum/job/job, level)
	if(!job || !level)
		return 0
	switch(job.department_flag)
		if(CIVILIAN)
			switch(level)
				if(1)
					return job_civilian_high
				if(2)
					return job_civilian_med
				if(3)
					return job_civilian_low
		if(SCI)
			switch(level)
				if(1)
					return job_sci_high
				if(2)
					return job_sci_med
				if(3)
					return job_sci_low
		if(MED)
			switch(level)
				if(1)
					return job_med_high
				if(2)
					return job_med_med
				if(3)
					return job_med_low
		if(SEC)
			switch(level)
				if(1)
					return job_sec_high
				if(2)
					return job_sec_med
				if(3)
					return job_sec_low
		if(ENG)
			switch(level)
				if(1)
					return job_eng_high
				if(2)
					return job_eng_med
				if(3)
					return job_eng_low
		if(CARGO)
			switch(level)
				if(1)
					return job_cargo_high
				if(2)
					return job_cargo_med
				if(3)
					return job_cargo_low
		if(SOLGOV)
			switch(level)
				if(1)
					return job_external_high
				if(2)
					return job_external_med
				if(3)
					return job_external_low
	return 0
