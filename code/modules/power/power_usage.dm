/// Indicates power usage levels for a single object
/datum/power_usage
	/// Idle power usage for stock parts
	var/base_idle
	/// Active power usage for stock parts
	var/base_active
	/// Power channel, only used for things that draw from APCs
	var/channel = AREA_USAGE_EQUIP

	/// This much usage will be applied to idle for every part rating
	var/list/idle_part_factors = list(
		/obj/item/stock_parts = 0,
		/obj/item/stock_parts/capacitor = 0,
		/obj/item/stock_parts/scanning_module = 0,
		/obj/item/stock_parts/manipulator = 0,
		/obj/item/stock_parts/micro_laser = 0,
		/obj/item/stock_parts/matter_bin = 0,
		/obj/item/stock_parts/cell/high = 0,
		/obj/item/stock_parts/electrolite = 0
	)

	/// This much usage will be applied to active for every part rating
	var/list/active_part_factors = list(
		/obj/item/stock_parts = 0,
		/obj/item/stock_parts/capacitor = 0,
		/obj/item/stock_parts/scanning_module = 0,
		/obj/item/stock_parts/manipulator = 0,
		/obj/item/stock_parts/micro_laser = 0,
		/obj/item/stock_parts/matter_bin = 0,
		/obj/item/stock_parts/cell/high = 0,
		/obj/item/stock_parts/electrolite = 0
	)

/datum/power_usage/New(idle = -1, active = -1, list/idle_part_factor_adjustments = null, list/active_part_factor_adjustments = null, channel = AREA_USAGE_EQUIP)
	if(active > idle && active != -1)
		CRASH("Higher idle power usage than active usage!")

	base_idle = idle
	base_active = active

	AdjustList(idle_part_factors, idle_part_factor_adjustments)
	AdjustList(active_part_factors, active_part_factor_adjustments)

	if (channel != AREA_USAGE_EQUIP)
		src.channel = channel

/datum/power_usage/AdjustList(stock_list, adjustments)
	if (!adjustments)
		return

	for (var/i in adjustments)
		var/value = adjustments[i]
		if (!isnum(value))
			CRASH("Invalid adjustment list: [json_encode(adjustments)]")

		if (!stock_list[i])
			CRASH("Invalid part adjustment: [i]")

		stock_list[i] = value

/datum/power_usage/proc/GetIdle(obj/machinery/machine)
	return CalcPower(machine, base_idle, idle_part_factors)

/datum/power_usage/proc/GetActive(obj/machinery/machine)
	return CalcPower(machine, base_active, active_part_factors)

/datum/power_usage/proc/CalcPower(obj/machinery/machine, base_value, list/part_factors)
	. = base_value
	if(. == -1)
		return 0

	for (var/i in machine.component_parts)
		var/obj/item/stock_parts/part = i
		var/part_type = part.type

		for (var/j in part_factors)
			if (ispath(part_type, j))
				var/rating_factor = part_factors[j]
				. += rating_factor * part.rating
				break
