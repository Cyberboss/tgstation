/obj/docking_port/mobile/crew
    name = "Crew Shuttle"
    id = "crew_shuttle"
    
	ignitionTime = 0    //first launch is instant
    callTime = INFINITY

    ignore_if_already_docked = TRUE

/obj/docking_port/mobile/crew/Initialize()
    . = ..()
    Launch()

/obj/docking_port/mobile/crew/Launch()
    request(SSshuttle.getDock(id))
    ignitionTime = 50
