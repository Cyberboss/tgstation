/*
	Defined in code/modules/mob/inventory.dm
*/

/mob/put_in_hand(obj/item/I, hand_index)
	var/obj/item/bodypart/B = has_active_hand()
	if(B && B.broken && !B.splinted)
		src << "<span class='danger'>Your [B.name] hurts badly as you try to pick up \the [I]!</span>"
		emote("scream")
		return

	..()