namespace Availpro.RateCategorisation.Parsing

module MealPlanParsing =
    open Core

    type MealPlan =
        | BreakfastIncluded
        | BreakfastNotIncluded

    let pBreakfast v =                   
        parseAny            
            [ //English
              "breakfast"
              "include breakfast"
              "includes breakfast"
              "including breakfast"
              "breakfast included"
              "breakfast is included"
              "with breakfast"
              //Spanish
              "desayuno incluido"
              "incluye el desayuno"
              //Portuguese
              "inclui pequeno-almoco"
              "pequeno-almoco incluido"
              "pequeno almoco incluido"
              "pequeno-almoco esta incluido"
              "pequeno almoco esta incluido" 
              //Icelandic
              "innifalið morgunverðarhlaðborð" ]
            v
    
    let parseBreakfast = pBreakfast BreakfastIncluded

    let pNotBreakfast v = 
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

    let parseNotBreakfast = pNotBreakfast BreakfastNotIncluded
    
    open Core.Operators

    let parseAll = 
        parseNotBreakfast         
        <|> parseBreakfast

    let parse = run parseAll

    [<CompiledName("IsMatch")>]
    let isMatch f text =
        match run f text with
        | Some _ -> true
        | None -> false