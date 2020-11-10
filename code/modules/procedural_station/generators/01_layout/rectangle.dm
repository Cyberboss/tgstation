/datum/procedural_station/generator/layout/rectangle/generate()
	// 50 is pure box
	// note we want overall area to remain the same regardless of width/height
	var/x_factor = rand(0.25, 0.75)

	var/xy_verts = initial_section_verticies()

	var/max_width = xy_verts[3] - xy_verts[1]
	var/max_height = xy_verts[4] - xy_verts[2]

	// y=mx+b
	var/y_factor = -x_factor + 1

	var/true_width = max_width * x_factor
	var/true_height = max_height * y_factor

