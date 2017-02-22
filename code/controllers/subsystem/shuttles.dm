/datum/subsystem/shuttle
	var/emergencyCallAmount = 0		//how many times the escape shuttle was called

/datum/subsystem/shuttle/requestEvac(mob/user, call_reason)
	if(!emergency)
		WARNING("requestEvac(): There is no emergency shuttle, but the \
			shuttle was called. Using the backup shuttle instead.")
		if(!backup_shuttle)
			throw EXCEPTION("requestEvac(): There is no emergency shuttle, \
			or backup shuttle! The game will be unresolvable. This is \
			possibly a mapping error, more likely a bug with the shuttle \
			manipulation system, or badminry. It is possible to manually \
			resolve this problem by loading an emergency shuttle template \
			manually, and then calling register() on the mobile docking port. \
			Good luck.")
			return
		emergency = backup_shuttle

	if(world.time - round_start_time < config.shuttle_refuel_delay)
		var/datum/job/J
		world << J.title
		user << "The emergency shuttle is refueling. Please wait another [abs(round(((world.time - round_start_time) - config.shuttle_refuel_delay)/600))] minutes before trying again."
		return

	switch(emergency.mode)
		if(SHUTTLE_RECALL)
			user << "The emergency shuttle may not be called while returning to Centcom."
			return
		if(SHUTTLE_CALL)
			user << "The emergency shuttle is already on its way."
			return
		if(SHUTTLE_DOCKED)
			user << "The emergency shuttle is already here."
			return
		if(SHUTTLE_IGNITING)
			user << "The emergency shuttle is firing its engines to leave."
			return
		if(SHUTTLE_ESCAPE)
			user << "The emergency shuttle is moving away to a safe distance."
			return
		if(SHUTTLE_STRANDED)
			user << "The emergency shuttle has been disabled by Centcom."
			return

	call_reason = trim(html_encode(call_reason))

	if(length(call_reason) < CALL_SHUTTLE_REASON_LENGTH && seclevel2num(get_security_level()) > SEC_LEVEL_GREEN)
		user << "You must provide a reason."
		return

	var/area/signal_origin = get_area(user)
	var/emergency_reason = "\nNature of emergency:\n\n[call_reason]"
	var/security_num = seclevel2num(get_security_level())
	switch(security_num)
		if(SEC_LEVEL_GREEN)
			emergency.request(null, 2, signal_origin, html_decode(emergency_reason), 0)
		if(SEC_LEVEL_BLUE)
			emergency.request(null, 1, signal_origin, html_decode(emergency_reason), 0)
		else
			emergency.request(null, 0.5, signal_origin, html_decode(emergency_reason), 1) // There is a serious threat we gotta move no time to give them five minutes.

	log_game("[key_name(user)] has called the shuttle.")
	message_admins("[key_name_admin(user)] has called the shuttle.")