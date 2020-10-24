/datum/procedural_station/generator/single_z/generate()
	// Fill the place with space
	// we's a space station after all
	for(var/x in 1 to world.maxx)
		for(var/y in 1 to world.maxy)
			set_area(x, y, 1, /area/space)
			set_turf(x, y, 1, /turf/open/space/basic)
