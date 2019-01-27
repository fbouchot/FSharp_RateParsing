namespace Availpro.RateCategorisation.Parsing

module Core =
    type Parser<'a> = Parser of (string -> 'a option)

    let run (Parser p) text = p text

    let replacediacritics (x:string) = x.Replace("é", "e")

    let prune source target = (source:string).IndexOf(target, System.StringComparison.InvariantCultureIgnoreCase) <> -1

    let parseAny list v =
        Parser <| fun text ->
            if list |> List.exists (prune text) then Some v
            else None

    let combineWord a b =
        [ sprintf "%s %s" a b
          sprintf "%s %s" b a ]
    
    module Operators =
        let (<|>) f g = 
            fun text ->
                run f text 
                |> Option.orElse (run g text)    
            |> Parser