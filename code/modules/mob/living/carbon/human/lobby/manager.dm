/datum/lobby_manager
	var/list/lights = list()
	var/list/hub_spawners = list()
	var/list/wall_spawners = list()
	var/list/shutters = list()
	var/list/announcers = list()

	var/obj/docking_port/mobile/crew_shuttle

	var/process_started = FALSE
	var/process_complete = FALSE

/datum/lobby_manager/proc/BeginProcess()
	set waitfor = FALSE

	if(process_started)
		return
	process_started = TRUE

	//TODO dock crew shuttle

	for(var/I in announcers)
		var/obj/O = I
		O.say("We have arrived at [station_name()]. Crew assigned to this outpost please report to the back area of the shuttle immediately.")
	announcers.Cut()

	UNTIL(SSticker.timeLeft < 150)

	for(var/I in lights)
		var/turf/open/floor/light/lobby/L = I
		L.WarningSequence()

	UNTIL(SSticker.timeLeft< 50)

	for(var/I in 1 to shutters.len)
		if(I == shutters.len)
			var/obj/machinery/door/door = shutters[I]
			door.close()    //wait on the last one
		else
			INVOKE_ASYNC(shutters[I], /obj/machinery/door/proc/close)
	
	process_complete = TRUE
	
	sleep(30)

	for(var/I in lights)
		var/turf/open/floor/light/lobby/L = I
		L.Normalize()
		CHECK_TICK
	lights.Cut()

	sleep(30)

	//TODO undock crew shuttle

	sleep(30)

	for(var/I in hub_spawners)
		new /obj/machinery/teleport/hub/lobby(get_turf(I))
		CHECK_TICK
	QDEL_LIST(hub_spawners)

	for(var/I in wall_spawners)
		var/turf/T = get_turf(I)
		qdel(I)
		T.PlaceOnTop(T)
		CHECK_TICK
	QDEL_LIST(wall_spawners)

	for(var/I in 1 to shutters.len)
		if(I == shutters.len)
			var/obj/machinery/door/door = shutters[I]
			door.open()    //wait on the last one
		else
			INVOKE_ASYNC(shutters[I], /obj/machinery/door/proc/open)
	shutters.Cut()
