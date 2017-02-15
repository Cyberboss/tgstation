//Main smess so we only need ONE for the station ~rj
/obj/machinery/power/smes/main
	charge = 20e6

/obj/machinery/power/smes/main/New()
	..()
	//Long ive ugly workarounds
	for(var/obj/item/weapon/stock_parts/capacitor/CP in component_parts)
		CP.rating = 5
	for(var/obj/item/weapon/stock_parts/cell/PC in component_parts)
		PC.rating = 5
	RefreshParts()