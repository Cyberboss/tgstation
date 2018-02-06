/mob/living/carbon/human/lobby/Life()
	return

/mob/living/carbon/human/lobby/Stat()
	..()
	LobbyStat()

/mob/living/carbon/human/lobby/forceMove(atom/destination)
	loc = destination

/mob/living/carbon/human/lobby/say(message)
	client.ooc(message)