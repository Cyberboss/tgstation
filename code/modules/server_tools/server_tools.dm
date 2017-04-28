GLOBAL_VAR_INIT(reboot_mode, REBOOT_MODE_NORMAL)	//if the world should request the service to kill it at reboot
GLOBAL_PROTECT(reboot_mode)

/world/proc/RunningService()
	return params[SERVER_SERVICE_PARAM]

/world/proc/ExportService(command)
	shell("python tools/nudge.py \"[command]\"")

/world/proc/IRCBroadcast(msg)
	ExportService("irc [msg]")

/world/proc/ServiceReboot()
	switch(GLOB.reboot_mode)
		if(REBOOT_MODE_HARD)
			to_chat(src, "<span class='boldannounce'>Hard reboot triggered, you will automatically reconnect...</span>")
		if(REBOOT_MODE_SHUTDOWN)
			to_chat(src, "<span class='boldannounce'>The server is shutting down...</span>")
	log_world("Sending shutdown request!");\
	sleep(1)	//flush the buffers
	ExportService("killme")	//should not return, EVAH

/world/proc/ServiceCommand(list/params)
	var/sCK = RunningService()
	var/their_sCK = params["serviceCommsKey"]

	if(!their_sCK || their_sCK != sCK)
		return "Invalid comms key!";

	var/command = params["command"]
	if(!command)
		return "No command!"
	
	switch(command)
		if("hard_reboot")
			if(GLOB.reboot_mode != REBOOT_MODE_HARD)
				GLOB.reboot_mode = REBOOT_MODE_HARD
				log_world("Hard reboot requested by service")
				message_admins("The world will hard reboot at the end of the game. Requested by service.")
				SSblackbox.set_val("service_hard_restart", TRUE)
		if("graceful_shutdown")
			if(GLOB.reboot_mode != REBOOT_MODE_SHUTDOWN)
				GLOB.reboot_mode = REBOOT_MODE_SHUTDOWN
				log_world("Shutdown requested by service")
				message_admins("The world will shutdown at the end of the game. Requested by service.")
				SSblackbox.set_val("service_shutdown", TRUE)
		if("world_announce")
			var/msg = params["message"]
			if(!istext(msg) || !msg)
				return "No message set!"
			to_chat(src, "<span class='boldannounce'>[msg]</span>")
		if("irc_check")
			return "[clients.len] players on [SSmapping.config.map_name], Mode: [GLOB.master_mode]; Round [SSticker.HasRoundStarted() ? (SSticker.IsRoundInProgress() ? "Finishing" : "Active") : "Starting"] -- [config.server ? config.server : "byond://[address]:[port]"]" 
		else
			return "Unknown command: [command]"

