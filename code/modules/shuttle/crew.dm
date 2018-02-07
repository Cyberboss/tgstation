/obj/docking_port/mobile/crew
	name = "Crew Shuttle"
	id = "crew_shuttle"
	
	ignitionTime = 0    //first launch is instant
	callTime = INFINITY

	ignore_already_docked = TRUE

/obj/docking_port/mobile/crew/Initialize()
	. = ..()
	Launch()

/obj/docking_port/mobile/crew/proc/Launch()
	request(SSshuttle.getDock(id))
	ignitionTime = 50

/obj/docking_port/mobile/crew/proc/StopFlying()
	setTimer(0)
