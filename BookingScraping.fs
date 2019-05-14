namespace Availpro.RateCategorisation.Parsing

module BookingScraping =
    open FSharp.Data
    open System.Text.RegularExpressions
    open FSharp.Core
    open Availpro.RateCategorisation.Parsing
    open MealPlanParsing


    let scrap url =
        Http.RequestString(
            url, 
            httpMethod="GET", 
            headers=[("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36")])   
            
//    // Path functions
//    let root = __SOURCE_DIRECTORY__
//    let (</>) x y = System.IO.Path.Combine(x, y)

//    let BookingUrl = "https://www.booking.com/hotel/es/vincci-gala.en-gb.html?checkin=2019-03-01&checkout=2019-03-02&group_adults=2&selected_currency=EUR&sb_price_type=total&type=total&do_availability_check=1"

//    let HtmlResponse = 
//        Http.RequestString(
//            BookingUrl, 
//            httpMethod="GET", 
//            headers=[("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36")])

//    let samplePath = root </> "Html/demo_20190514.html"
//    System.IO.File.WriteAllText(samplePath, HtmlResponse)

//    let html = System.IO.File.ReadAllText(samplePath)

//    let jsonText = 
//        let m = Regex.Match(html, "b_rooms_available_and_soldout\s*[:=]\s*(?<roomAvailability>\[.+\}\]),")    
//        if m.Success then Some m.Groups.["roomAvailability"].Value
//        else None

//    printfn "%s" jsonText.Value

//    let jsonPath = root </> "Html/demo_20180129.json"
//    System.IO.File.WriteAllText(jsonPath, jsonText.Value)

//    let json = System.IO.File.ReadAllText(jsonPath)

//    //printfn "%s" json

//    type RoomType = JsonProvider<"Html/demo_20180128.json", RootName = "RoomType">

//    type PaymentType =
//             PayNow
//           | PayLater
//           | Other

//    let pPaymentType (paymentType : int option) =
//        match paymentType with
//        | Some(paymentType) -> match paymentType with 
//            | 0 -> PayNow
//            | 1 -> PayLater
//            | _ -> Other
//        | None -> Other

//    type CateringType = 
//        RoomOnly
//        |BreakfastIncluded
//        |HalfBoard
//        |FullBoard
//        |AllInclusive

//    // parse tring option to string
//    let parseMealPlanOption (meal : string option) =
//        match meal with
//        | Some(meal) -> meal
//        | None -> "Breakfast not Included"
      
//    type Rate =
//        { Id: string    
//          Price: decimal
//          PaymentType: PaymentType
//          MealPlan: MealPlanParsing.MealPlan option}

//    type Room = 
//        { Id : int
//          Name: string
//          Rates: Rate[]}


//    let searchResult = RoomType.Parse(json)


//    //searchResult.[0].BBlocks.[2].BMealplanIncludedName

//    let rooms = searchResult |> Array.map (fun room -> 
//        { Id = room.BId
//          Name = room.BName
//          Rates = room.BBlocks |> Array.map (fun rate -> 
//          { Id = rate.BBlockId
//            Price = rate.BRawPrice
//            PaymentType = rate.BBookNowPayLater |> pPaymentType
//            MealPlan = rate.BMealplanIncludedName |> parseMealPlanOption |> MealPlanParsing.parse
//            }) 
//         })






////printfn "%A" rooms
////(*As meal plan name in the json porpety it's some time string and some times null, the Jsonrovider Parser infers his type as string option
////As I understand when in F# they put option types it's like nullable types in C#.
////So in this case we need to pas the meal plan that it's type string option to a function that checks if itr's string or NONE, the function it's parseMealPlanOption
////that return the original string if it is an string or if it was a null I return a string "breakfast not included". Then after the function we pass from type string option to type string.*)
