/// Fills the map with a specific area and turf type
/datum/procedural_station/generator/backdrop
	var/area_type
	var/turf_type

/datum/procedural_station/generator/backdrop/generate()
	if(!ispath(area_type, /area))
		CRASH("Invalid area_type!")
	if(!ispath(turf_type, /turf))
		CRASH("Invalid turf_type!")

	var/gen_area = area_type != world.area
	var/gen_turf = turf_type != world.turf
	if(!gen_area && !gen_turf)
		// no-op
		return

	for(var/x in 1 to world.maxx)
		for(var/y in 1 to world.maxy)
			if(gen_turf)
				set_turf(x, y, 1, turf_type)
			if(gen_area)
				set_area(x, y, 1, area_type)
