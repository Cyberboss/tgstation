/datum/procedural_station/generator/layout/proc/initial_section_verticies()
	RETURN_TYPE(/list)

	var/map_size_factor
	switch(parameters.station_size)
		if(PS_SIZE_SMALL)
			map_size_factor = 0.5
		if(PS_SIZE_MED)
			map_size_factor = 0.7
		if(PS_SIZE_LARGE)
			map_size_factor = 0.9

	var/minx = FLOOR((1 - map_size_factor) * world.maxx / 2)
	var/maxx = world.maxx - minx
	var/miny = FLOOR((1 - map_size_factor) * world.maxy / 2)
	var/maxy = world.maxy - miny

	return list(minx, miny, maxx, maxy)
