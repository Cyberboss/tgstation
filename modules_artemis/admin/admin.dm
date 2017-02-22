//Jury rigged version of freeze from other servers ~rj
/client/proc/cmd_admin_restrain(mob/living/M in mob_list)
	set category = "Admin"
	set name = "Restrain"

	if(!holder)
		src << "<font color='red'>Error: Admin-PM-Context: Only administrators may use this command.</font>"
		return
	if(!M || !ismob(M))
		usr << "You seem to be selecting a mob that doesn't exist anymore or has never existed."
		return

	if(istype(M, /mob/living/carbon) || istype(M, /mob/living/silicon))
		if(M.a_restrained == 1)
			M.a_restrained = 0
			M.canmove = 1
			M.stunned = 0
			message_admins("[key_name_admin(src)] has lifted [key_name(M)]'s restraining order.")
			log_admin("[key_name_admin(usr)] has lifted [key_name(M)]'s restraining order.")
		else
			M.a_restrained = 1
			M.canmove = 0
			M.stunned = 900000
			message_admins("[key_name_admin(src)] has restrained [key_name(M)].")
			log_admin("[key_name_admin(src)] has restrained [key_name(M)].")

/mob/living
	var/a_restrained = 0

/client
	var/see_attack_notice = 1

/client/proc/toggle_attack_notice()
	set category = "Admin"
	set name = "Toggle Attack Notice"
	if(!holder)
		src << "<font color='red'>Error: Only administrators may use this command.</font>"
		return
	see_attack_notice = !see_attack_notice
	usr << "Attack notice toggled [see_attack_notice ? "on" : "off"]"