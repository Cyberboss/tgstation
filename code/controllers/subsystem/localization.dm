/datum/controller/subsystem/localization
	name = "Localization"
	flags |= SS_NO_FIRE
	init_order = INIT_ORDER_LOCALIZATION

	//static for sanic + no recovery
	var/static/list/language_mappings

/datum/controller/subsystem/localization/PreInit()
	var/datum/controller/exclude_these = new
	var/list/exclude_vars = exclude_these.vars + list(NAMEOF(src, language_mappings))

	var/default_mappings = list()

	for(var/I in vars)
		if(I in exclude_vars)
			continue
		default_mappings += I
		vars[I] = default_mappings.len

	language_mappings = list(LOCALE_DEFAULT = default_mappings)

/datum/controller/subsystem/localization/proc/LoadLanguage(lang_identifier)
	if(language_mappings[lang_identifier])
		CRASH("LoadLanguage called twice for [lang_identifier]")
	var/base_langs_path = "strings/languages/[lang_identifier]"
	. = list()
	for(var/I in flist(base_langs_path))
		. += json_decode(file2text(I))
	language_mappings[lang_identifier] = .

/datum/text_instance
	var/key
	var/list/formatters
	var/list/preformatters

/datum/text_instance/New(_key, ...)
	key = _key
	if(args.len <= 1 || istext(key))
		return
	
	var/list/_formatters = args[args.len] == null ? args.Copy(2, args.len - 1) : args.Copy(2)
	var/list/_preformatters
	for(var/I in 1 to _formatters.len)
		var/datum/thing = _formatters[I]
		if(thing && istype(thing))
			var/datum/weak_reference/weak = WEAKREF(thing)
			var/quick_formatted = "[thing]"
			if(!weak)
				//deleted, format it NAOW
				_formatters[I] = quick_formatted
			else
				_formatters[I] = weak
				LAZYADD(preformatters, quick_formatted)    

/datum/text_instance/proc/Format(lang_identifier = LOCALE_DEFAULT)
	var/list/_formatters = formatters
	var/list/_preformatters = preformatters
	var/_key = key
	var/entry
	if(istext(_key))
		entry = _key
	else
		var/list/language = SSlocalization.language_mappings[lang_identifier]
		if(!language)
			language = SSlocalization.LoadLanguage(lang_identifier)
		entry = language[_key]
	var/list/formatted_formatters
	if(_formatters)
		formatted_formatters.len = _formatters.len + 1
		var/preformatter_count
		for(var/I in 1 to _formatters.len)
			var/formatter = _formatters[I]
			if(!istext(formatter))
				if(istype(formatter, /datum/weak_reference))
					var/datum/weak_reference/weak = formatter
					var/datum/thing = weak.Resolve()
					formatter = thing ? thing : preformatters[++preformatter_count]
				else
					var/datum/text_instance/inner = formatter
					formatter = inner.Format(lang_identifier)
			formatted_formatters[I + 1] = formatter
		formatted_formatters[1] = entry
	else
		formatted_formatters = list(entry)
	return text(arglist(formatted_formatters))
