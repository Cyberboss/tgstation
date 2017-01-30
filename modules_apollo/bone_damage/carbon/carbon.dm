/*
	Defined in code/modules/mob/living/carbon/carbon.dm
*/

/mob/living/carbon/fully_heal(admin_revive = 0)
	for(var/X in get_fractures())
		var/obj/item/bodypart/B = X
		B.heal_fracture()
	..()

// fracture-related procs
/mob/living/carbon/proc/get_fractures()
	. = list()
	for(var/X in bodyparts)
		var/obj/item/bodypart/B = X
		if(B.broken)
			. += B