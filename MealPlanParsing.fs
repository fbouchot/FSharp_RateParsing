namespace Availpro.RateCategorisation.Parsing

module MealPlanParsing =
    open Core
    open Core.Operators

    type MealPlan =
        | BreakfastIncluded
        | BreakfastNotIncluded
    
    /// English
    let pEnBreakfast v = 
        v |> parseAny 
            [ "breakfast"
              "include breakfast"
              "includes breakfast"
              "including breakfast"
              "breakfast included"
              "breakfast is included"
              "with breakfast" ]
    
    let pEsBreakfast v = 
        v |> parseAny
            [ "desayuno incluido"
              "incluye el desayuno" ]
    
    let pPtBreakfast v = 
        v |> parseAny
            [ "inclui pequeno-almoco"
              "pequeno-almoco incluido"
              "pequeno almoco incluido"
              "pequeno-almoco esta incluido"
              "pequeno almoco esta incluido" ]
    
    let pIsBreakfast v = 
        v |> parseAny [ "innifalið morgunverðarhlaðborð" ]
    
    //Compose lang by lang
    let pBreakfast v = 
        pEnBreakfast v 
        <|> pEsBreakfast v
        <|> pPtBreakfast v
        <|> pIsBreakfast v
    
    let parseBreakfast = pBreakfast BreakfastIncluded

    //Compose all lang at one
    let pNotIncludedBreakfast v = 
        parseAny
            [ //English
              "breakfast excluded"
              "breakfast is excluded"
              "breakfast is not included"
              "breakfast not included"
              "without breakfast"
              "room only"
              "no meals included"
              //Icelandic
              "morgunverður er ekki innifalinn" ]
            v

    let parseNotBreakfast = pNotIncludedBreakfast BreakfastNotIncluded
    
    let parseAll = 
        parseNotBreakfast         
        <|> parseBreakfast

    let parse = run parseAll

    [<CompiledName("IsMatch")>]
    let isMatch f text =
        match run f text with
        | Some _ -> true
        | None -> false