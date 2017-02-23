/obj/machinery/portable_atmospherics/canister/ui_act(action, params)
	if(..())
		return
	switch(action)
		if("relabel")
			var/label = input("New canister label:", name) as null|anything in label2types
			if(label && !..())
				var/newtype = label2types[label]
				if(newtype)
					var/obj/machinery/portable_atmospherics/canister/replacement = new newtype(loc, air_contents)
					if(connected_port)
						replacement.connected_port = connected_port
						replacement.connected_port.connected_device = replacement
					replacement.interact(usr)
					qdel(src)
		if("pressure")
			var/pressure = params["pressure"]
			if(pressure == "reset")
				pressure = CAN_DEFAULT_RELEASE_PRESSURE
				. = TRUE
			else if(pressure == "min")
				pressure = CAN_MIN_RELEASE_PRESSURE
				. = TRUE
			else if(pressure == "max")
				pressure = CAN_MAX_RELEASE_PRESSURE
				. = TRUE
			else if(pressure == "input")
				pressure = input("New release pressure ([CAN_MIN_RELEASE_PRESSURE]-[CAN_MAX_RELEASE_PRESSURE] kPa):", name, release_pressure) as num|null
				if(!isnull(pressure) && !..())
					. = TRUE
			else if(text2num(pressure) != null)
				pressure = text2num(pressure)
				. = TRUE
			if(.)
				release_pressure = Clamp(round(pressure), CAN_MIN_RELEASE_PRESSURE, CAN_MAX_RELEASE_PRESSURE)
				investigate_log("was set to [release_pressure] kPa by [key_name(usr)].", "atmos")
		if("valve")
			var/logmsg
			valve_open = !valve_open
			if(valve_open)
				logmsg = "Valve was <b>opened</b> by [key_name(usr)], starting a transfer into \the [holding || "air"].<br>"
				if(!holding)
					var/plasma = air_contents.gases["plasma"]
					var/n2o = air_contents.gases["n2o"]
					var/bz = air_contents.gases["bz"]
					var/freon = air_contents.gases["freon"]
					if(n2o || plasma || bz || freon)
						message_admins("[key_name_admin(usr)] (<A HREF='?_src_=holder;adminmoreinfo=\ref[usr]'>?</A>) (<A HREF='?_src_=holder;adminplayerobservefollow=\ref[usr]'>FLW</A>) opened a canister that contains the following: (<A HREF='?_src_=holder;adminplayerobservecoodjump=1;X=[x];Y=[y];Z=[z]'>JMP</a>)")
						log_admin("[key_name(usr)] opened a canister that contains the following at [x], [y], [z]:")
						if(plasma)
							log_admin("Plasma")
							message_admins("Plasma")
						if(n2o)
							log_admin("N2O")
							message_admins("N2O")
						if(bz)
							log_admin("BZ Gas")
							message_admins("BZ Gas")
						if(freon)
							log_admin("Freon")
							message_admins("Freon")
			else
				logmsg = "Valve was <b>closed</b> by [key_name(usr)], stopping the transfer into \the [holding || "air"].<br>"
			investigate_log(logmsg, "atmos")
			release_log += logmsg
			. = TRUE
		if("eject")
			if(holding)
				if(valve_open)
					//GOD DAMN AUTO CLOSE THAT FUCKING VALVE! ~rj
					valve_open = !valve_open
					//investigate_log("[key_name(usr)] removed the [holding], leaving the valve open and transfering into the <span class='boldannounce'>air</span><br>", "atmos")
				holding.loc = get_turf(src)
				holding = null
				. = TRUE
	update_icon()