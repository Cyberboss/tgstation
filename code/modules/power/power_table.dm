/datum/power_table
	/// List of /datum/power_usage keyed by owning type. Keep sorted descending by most prominent usage type (at T1 parts) for organization. THIS IS NOT A TYPECACHE, every subtype that uses power must be defined here.
	var/list/entries = list(
		// producers
		/obj/machinery/power/rtg/abductor = new /datum/power_usage(active_power_adjustments = list(/obj/machinery/stock_parts = 20000), active = 0),
		/obj/machinery/power/rtg/advanced = new /datum/power_usage(active_power_adjustments = list(/obj/machinery/stock_parts = 1250), active = 0),
		/obj/machinery/power/rtg = new /datum/power_usage(active_power_adjustments = list(/obj/machinery/stock_parts = 1000), active = 0),

		// consumers
		/obj/machinery/cell_charger = new /datum/power_usage(active_power_adjustments = list(/obj/machinery/stock_parts/capacitor = 250), idle = 10),
		/obj/machinery/emitter = new /datum/power_usage(active = 350, active_power_adjustments = list(/obj/machinery/stock_parts/manipulator = -50), idle = 10),
		/obj/machinery/computer/crew = new /datum/power_usage(idle = 500),
		/obj/machinery/computer/aifixer = new /datum/power_usage(idle = 300, active = 1000),
		/obj/machinery/computer = new /datum/power_usage(idle = 300),
		/obj/machinery/autolathe = new /datum/power_usage(active = 100, idle = 10),
		/obj/machinery/computer/bank_machine = new /datum/power_usage(idle = 100),
		/obj/machinery/announcement_system = new /datum/power_usage(idle = 20, active = 50),
		/obj/machinery/computer/scan_consolenew = new /datum/power_usage(idle = 10, active = 400),
		/obj/machinery/camera = new /datum/power_usage(idle = 10),
		/obj/machinery/button = new /datum/power_usage(idle = 2, active = 5, channel = AREA_USAGE_ENVIRON),
		/obj/machinery/airlock_sensor = new /datum/power_usage(idle = 10, channel = AREA_USAGE_ENVIRON)
	)
