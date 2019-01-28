namespace Availpro.RateCategorisation.Parsing

module Core =
    type Parser<'a> = Parser of (string -> 'a option)

    let run (Parser p) text = p text

    let replaceDiacritics (x:string) = 
        x
        |> System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes
        |> System.Text.Encoding.UTF8.GetString

    let icontains source target = (source:string).IndexOf(target, System.StringComparison.InvariantCultureIgnoreCase) <> -1

    let parseAny list v =
        Parser <| fun text ->
            if list |> List.exists (icontains text) then Some v
            else None

    module Operators =
        let (<|>) f g = 
            fun text ->
                run f text 
                |> Option.orElse (run g text)    
            |> Parser