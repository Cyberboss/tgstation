/datum/lobby_manager
	var/list/lights = list()
	var/list/hub_spawners = list()
	var/list/wall_spawners = list()
	var/list/shutters = list()

	var/obj/docking_port/mobile/crew_shuttle

	var/process_started = FALSE
	var/process_complete = FALSE

/datum/lobby_manager/proc/BeginProcess()
	set waitfor = FALSE

	if(process_started)
		return
	process_started = TRUE

	//TODO dock crew shuttle

	UNTIL(SSticker.timeLeft < 150)

	for(var/I in lights)
		var/turf/open/floor/light/lobby/L = I
		L.WarningSequence()

	UNTIL(SSticker.timeLeft< 50)

	for(var/I in 1 to shutters.len)
		if(I == shutters.len)
			var/obj/machinery/door/door = I
			door.close()    //wait on the last one
		else
			INVOKE_ASYNC(I, /obj/machinery/door/proc/close)
	
	process_complete = TRUE
	
	for(var/I in lights)
		var/turf/open/floor/light/lobby/L = I
		L.Normalize()
		CHECK_TICK

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
			var/obj/machinery/door/door = I
			door.open()    //wait on the last one
		else
			INVOKE_ASYNC(I, /obj/machinery/door/proc/open)
