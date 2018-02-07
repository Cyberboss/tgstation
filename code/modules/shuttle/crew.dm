/obj/docking_port/mobile/crew
	name = "Crew Shuttle"
	id = "crew_shuttle"
	
	timid = FALSE
	preferred_direction = WEST
	port_direction = SOUTH

	ignitionTime = 0    //first launch is instant
	callTime = INFINITY

	ignore_already_docked = TRUE

/obj/docking_port/mobile/crew/Initialize()
	..()
	return INITIALIZE_HINT_LATELOAD

/obj/docking_port/mobile/crew/LateInitialize()
	..()
	Launch()

/obj/docking_port/mobile/crew/proc/Launch()
	request(SSshuttle.getDock(id))
	ignitionTime = 50

/obj/docking_port/mobile/crew/proc/StopFlying()
	setTimer(0)
