/mob/living/carbon/human/lobby/Login()
	if(CONFIG_GET(flag/use_exp_tracking))
		client.set_exp_from_db()
		client.set_db_player_flags()
	if(!mind)
		mind = new /datum/mind(key)
		mind.active = TRUE
		mind.current = src

	..()
	
	var/motd = global.config.motd
	if(motd)
		to_chat(src, "<div class=\"motd\">[motd]</div>")

	if(GLOB.admin_notice)
		to_chat(src, "<span class='notice'><b>Admin Notice:</b>\n \t [GLOB.admin_notice]</span>")

	var/spc = CONFIG_GET(number/soft_popcap)
	if(spc && living_player_count() >= spc)
		to_chat(src, "<span class='notice'><b>Server Notice:</b>\n \t [CONFIG_GET(string/soft_popcap_message)]</span>")

	client.playtitlemusic()
	if(SSticker.current_state < GAME_STATE_SETTING_UP)
		var/tl = SSticker.GetTimeLeft()
		var/postfix
		if(tl > 0)
			postfix = "in about [DisplayTimeText(tl)]"
		else
			postfix = "soon"
		to_chat(src, "Please set up your character using a console on the left and enter the green area to indicate your readiness.")
		to_chat(src, "The game will start [postfix].")

	show_player_polls.CheckDB()
	client.prefs.copy_to(src)
	name = client.key
	var/round_started = SSticker.HasRoundStarted()
	splash_screen = new(client, TRUE, round_started)
	if(SSticker.IsPreGame() && !round_started) //post initializations
		OnInitializationsComplete()
	SSticker.OnRoundstart(roundstart_callback)

/mob/living/carbon/human/lobby/Logout()
	..()
	if(!new_character)//Here so that if they are spawning and log out, the other procs can play out and they will have a mob to come back to.
		PhaseOut()