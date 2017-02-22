//For attack logs
/obj/item/attack(mob/living/M, mob/living/user)
	var/turf/attack_location = get_turf(user)
	var/what_done = "attacked"
	var/mob/living/target = M
	var/mob/living/living_target
	var/object = src.name
	var/addition = "(INTENT: [uppertext(user.a_intent)]) (DAMTYPE: [uppertext(damtype)])"

	if(target && isliving(target))
		living_target = target

	if(user)
		var/message = "\[[time_stamp()]\] <font color='red'>[user ? "[user.name][(user.ckey) ? "([user.ckey])" : ""]" : "NON-EXISTANT SUBJECT"] has [what_done] [target ? "[target.name][(target && target.ckey) ? "([target.ckey])" : ""]" : "NON-EXISTANT SUBJECT"][object ? " with [object]" : " "][addition][(living_target) ? " (NEWHP: [living_target.health])" : ""][(attack_location) ? "([attack_location.x],[attack_location.y],[attack_location.z])" : ""]</font>"
		message += "(<A HREF='?_src_=holder;adminmoreinfo=\ref[user]'>?</A>)(<A HREF='?_src_=holder;adminplayerobservecoodjump=1;X=[user.x];Y=[user.y];Z=[user.z]'>JMP</a>)"
		for(var/client/C in admins)
			if (C.see_attack_notice)
				C << message
	..(M, user)