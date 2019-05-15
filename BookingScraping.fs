namespace Availpro.RateCategorisation.Parsing

module BookingScraping =
    open FSharp.Data
    open System.Text.RegularExpressions
    open FSharp.Core
    open Availpro.RateCategorisation.Parsing
    open MealPlanParsing

    //let BookingUrl = "https://www.booking.com/hotel/es/vincci-gala.en-gb.html?checkin=2019-05-15&checkout=2019-05-16&group_adults=2&selected_currency=EUR&sb_price_type=total&type=total&do_availability_check=1"

    type RoomType = JsonProvider<"Html/demo_20190515.json", RootName = "RoomType">

    type PaymentType =
           PayNow
           | PayLater
           | Other

    type CateringType = 
        RoomOnly
        |BreakfastIncluded
        |HalfBoard
        |FullBoard
        |AllInclusive

    type Rate =
        { Id: string    
          Price: decimal
          PaymentType: PaymentType
          MealPlan: MealPlanParsing.MealPlan option}
    
    type Room = 
        { Id : int
          Name: string
          Rates: Rate[]}



    let pPaymentType (paymentType : int option) =
        match paymentType with
        | Some(paymentType) -> match paymentType with 
            | 0 -> PayNow
            | 1 -> PayLater
            | _ -> Other
        | None -> Other

    // parse string option to string
    let parseMealPlanOption (meal : string option) =
        match meal with
        | Some(meal) -> meal
        | None -> "Breakfast not Included"

    let parseRateIdOption (rateId : string option) =
        match rateId with
        | Some(rateId) -> rateId
        | None -> "no rate id"

    let parsePriceOption (price : decimal option) =
        match price with
        | Some(price) -> price
        | None -> (decimal) 0

      
    let scrap url =        
        let content = Http.RequestString(
            "https://www.booking.com/hotel/es/vincci-gala.en-gb.html?checkin=2019-05-15&checkout=2019-05-16&group_adults=2&selected_currency=EUR&sb_price_type=total&type=total&do_availability_check=1",
            httpMethod="GET", 
            headers=[("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36")])
            
        // Path functions
        let root = __SOURCE_DIRECTORY__
        let (</>) x y = System.IO.Path.Combine(x, y)

        let samplePath = root </> "Html/demo_20190515.html"
        System.IO.File.WriteAllText(samplePath, content)
        //let content = System.IO.File.ReadAllText(samplePath)
        let jsonText = 
            let m = Regex.Match(content, "b_rooms_available_and_soldout\s*[:=]\s*(?<roomAvailability>\[.+\}\]),")    
            if m.Success then Some m.Groups.["roomAvailability"].Value
            else None

        //printfn "%s" jsonText.Value
        let jsonPath = root </> "Html/demo_20190515.json"
        System.IO.File.WriteAllText(jsonPath, jsonText.Value)
        //let json = System.IO.File.ReadAllText(jsonPath)

        let json = jsonText.Value
        //printfn "%s" json

        let searchResult = RoomType.Parse(json)
        let rooms = searchResult |> Array.map (fun room -> 
            { Id = room.BId
              Name = room.BName
              Rates = room.BBlocks |> Array.map (fun rate ->
              { Id = rate.BBlockId |> parseRateIdOption
                Price = rate.BRawPrice |> parsePriceOption
                PaymentType = rate.BBookNowPayLater |> pPaymentType
                MealPlan = rate.BMealplanIncludedName |> parseMealPlanOption |> MealPlanParsing.parse
                }) 
             })
        rooms

//    //searchResult.[0].BBlocks.[2].BMealplanIncludedName



////printfn "%A" rooms
////(*As meal plan name in the json porpety it's some time string and some times null, the Jsonrovider Parser infers his type as string option
////As I understand when in F# they put option types it's like nullable types in C#.
////So in this case we need to pas the meal plan that it's type string option to a function that checks if itr's string or NONE, the function it's parseMealPlanOption
////that return the original string if it is an string or if it was a null I return a string "breakfast not included". Then after the function we pass from type string option to type string.*)
