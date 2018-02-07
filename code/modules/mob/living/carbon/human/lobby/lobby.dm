//A special mob with permission to live before the round starts
/mob/living/carbon/human/lobby
	name = "Glitch in the Matrix"
	status_flags = GODMODE
	//no griff
	density = FALSE

	//we handle this
	notransform = TRUE

	var/prefers_observer = FALSE
	var/spawning = FALSE
	
	var/mob/living/carbon/human/new_character
	var/obj/screen/splash/splash_screen
	var/datum/callback/roundstart_callback
	
	var/datum/action/lobby/setup_character/setup_character
	var/datum/action/lobby/show_player_polls/show_player_polls

INITIALIZE_IMMEDIATE(/mob/living/carbon/human/lobby)

/mob/living/carbon/human/lobby/Initialize()
	. = ..()

	GLOB.alive_mob_list -= src
	GLOB.lobby_players += src

	MoveToStartArea()
	equipOutfit(/datum/outfit/vr_basic, FALSE)

	setup_character = new
	setup_character.Grant(src)
	show_player_polls = new
	show_player_polls.Grant(src)

	verbs += /mob/dead/proc/server_hop

	roundstart_callback = CALLBACK(src, .proc/OnRoundstart)

/mob/living/carbon/human/lobby/Destroy()
	QDEL_NULL(setup_character)
	QDEL_NULL(show_player_polls)
	LAZYREMOVE(SSticker.round_start_events, roundstart_callback)
	QDEL_NULL(roundstart_callback)
	QDEL_NULL(new_character)
	QDEL_NULL(splash_screen)
	GLOB.lobby_players -= src
	return ..()

/mob/living/carbon/human/lobby/proc/MoveToStartArea()
	if(loc)
		RunSparks()
	forceMove(pick(GLOB.newplayer_start))
	RunSparks()

/mob/living/carbon/human/lobby/proc/IsReady()
	return (client || new_character) && istype(get_area(src), /area/shuttle/lobby/start_zone)

/mob/living/carbon/human/lobby/proc/OnInitializationsComplete()
	QDEL_NULL(setup_character)
	QDEL_NULL(show_player_polls)
	window_flash(client, ignorepref = TRUE) //let them know lobby has opened up.
	if(become_observer.on)
		make_me_an_observer()
		return
	PhaseOutSplashScreen()

/mob/living/carbon/human/lobby/proc/OnRoundstart()
	if(!new_character)
		return
	
	var/mob/nc = new_character
	nc.notransform = TRUE
	addtimer(VARSET_CALLBACK(new_character, notransform, FALSE), 30, TIMER_CLIENT_TIME)
	transfer_character()
	PhaseOutSplashScreen(nc)

/mob/living/carbon/human/lobby/proc/PhaseOutSplashScreen(mob/character)
	splash_screen.Fade(TRUE, character != null)
	if(character)
		splash_screen = null
	else
		notransform = FALSE

/mob/living/carbon/human/lobby/proc/PhaseInSplashScreen()
	invisibility = INVISIBILITY_MAXIMUM
	RunSparks()
	notransform = TRUE
	splash_screen.Fade(FALSE, FALSE)

/mob/living/carbon/human/lobby/proc/RunSparks()
	do_sparks(5, FALSE, src)

/mob/living/carbon/human/lobby/proc/PhaseOut()
	RunSparks()
	key = null//We null their key before deleting the mob, so they are properly kicked out.
	qdel(src)
