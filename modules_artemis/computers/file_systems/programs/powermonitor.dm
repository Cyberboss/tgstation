/datum/computer_file/program/power_monitor
	var/list/apcs

/datum/computer_file/program/power_monitor/search()
	var/turf/T = get_turf(computer)
	attached = locate() in T

	apcs = new/list()
	for(var/obj/machinery/power/sensor/s in powergridsensors)
		if(s.powernet)
			for(var/obj/machinery/power/terminal/term in s.powernet.nodes)
				var/obj/machinery/power/apc/A = term.master
				if(istype(A))
					apcs += A

/datum/computer_file/program/power_monitor/ui_data()
	var/list/data = get_header_data()
	data["stored"] = record_size
	data["interval"] = record_interval / 10
	data["attached"] = attached ? TRUE : FALSE
	if(attached)
		data["supply"] = attached.powernet.viewavail
		data["demand"] = attached.powernet.viewload
	data["history"] = history

	data["areas"] = list()

	for(var/obj/machinery/power/apc/A in apcs)
		if(istype(A))
			data["areas"] += list(list(
				"name" = A.area.name,
				"charge" = A.cell.percent(),
				"load" = A.lastused_total,
				"charging" = A.charging,
				"eqp" = A.equipment,
				"lgt" = A.lighting,
				"env" = A.environ
			))

	return data