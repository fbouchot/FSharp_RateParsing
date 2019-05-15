#r "packages/FSharp.Data.3.0.0/lib/net45/FSharp.Data.dll"
#r "System.Net"
#load "Core.fs"
#load "MealPlanParsing.fs"

open FSharp.Data
open FSharp.Core
open System.Linq

type Span = 
    {
        Class : string 
        Text : string
    }

type RoomDetails = 
                {
                    Iteration: int
                    RoomId: string
                    Occupancy: int
                    RoomName: string
                    RateName: string
                    Price: string
                }

type Rate =
        {
         RateName: string
         Price: string
         Occupancy : int
        }

type Room = 
        {
            RoomId: string
            RoomName: string            
            Rate : Rate[]
        }

// Path functions
let root = __SOURCE_DIRECTORY__
let (</>) x y = System.IO.Path.Combine(x, y)

let BookingUrl = "https://www.booking.com/hotel/es/vincci-gala.en-gb.html?checkin=2019-05-30&checkout=2019-05-31&group_adults=2&selected_currency=EUR&sb_price_type=total&type=total&do_availability_check=1"

let HtmlResponse = 
    Http.RequestString(
        BookingUrl, 
        httpMethod="GET", 
        headers=[("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36")])

   

let samplePath = root </> "Html/demo_Final.html"
System.IO.File.WriteAllText(samplePath, HtmlResponse)

let doc = HtmlDocument.Load(samplePath)

let occupanciesNodes = doc.Descendants "td" 
                        |> Seq.where (fun td -> 
                                let attribute = td.AttributeValue "class"
                                attribute.Contains "hprt-table-cell-occupancy"
                                && attribute.Contains "wholesalers" <> true)
              
let pricesNodes = doc.Descendants"td" 
                        |> Seq.where (fun td -> 
                                let attribute = td.AttributeValue "class"
                                attribute.Contains "hprt-table-cell-price"
                                && attribute.Contains "wholesalers" <> true)                
                                                
let roomsNodes = doc.Descendants "tr" 
                    |> Seq.where(fun tr ->                 
                            let attribute = tr.AttributeValue "class"
                            not (attribute.Contains "hprt-cheapest-block-row")
                            && not (attribute.Contains "wholesalers")
                            && tr.AttributeValue "data-block-id" <> "")
        
let ratesNodes = doc.Descendants "td"
                    |> Seq.where (fun tr ->
                            let attribute = tr.AttributeValue "class"
                            attribute.Contains "hprt-table-cell-conditions"
                            && not (attribute.Contains("wholesalers")))
                            
let getOccupancy (node:HtmlNode) = 
        let occupancy = node.Descendants "i" 
                            |> Seq.where (fun node -> 
                                    let attribute = node.AttributeValue "class"
                                    attribute.Contains "bicon-occupancy") 
                            |> Seq.length                                             
        occupancy              

let getRoomName (node:HtmlNode) (roomId:string) =
        let roomNameNode = node.Descendants "td" 
                            |> Seq.where (fun n -> 
                                    let attribute = n.AttributeValue "class"
                                    attribute.Contains "hprt-table-cell-roomtype"
                                )
                            |> Seq.tryHead
                         
        if roomNameNode.IsNone 
        then ""
        else
            let spanNode = roomNameNode.Value.Descendants "span"                      
                            |> Seq.where (fun n ->
                                    let attribute = n.AttributeValue "class"
                                    attribute.Contains "hprt-roomtype-icon-link"
                                    && not (attribute.Contains "hprt-roomtype-icon-link__photo"))
                            |> Seq.tryHead
                             
            if spanNode.IsSome 
            then spanNode.Value.InnerText()
            else                             
            let nameNode = roomNameNode.Value.Descendants "a"
                                |> Seq.where (fun r -> r.AttributeValue "data-room-id" = roomId)
                                |> Seq.where (fun r-> r.InnerText() <> "")
                                |> Seq.tryLast
                                
            if nameNode.IsNone 
                then ""
            else
                nameNode.Value.InnerText()

let getPrice (node:HtmlNode) = 
            let priceText = node.Descendants "div"
                            |> Seq.where (fun n -> 
                                                let attribute = n.AttributeValue "class" 
                                                attribute.Contains "hprt-price-price")
                            |> Seq.tryHead
                         
            if priceText.IsNone
            then "0"
            else
            priceText.Value.InnerText()                            

let getRateName (node:HtmlNode) = 
            let spans = node.Descendants "span"
                        |> Seq.where (fun s ->
                                            let attribute = s.AttributeValue "class"
                                            attribute <> "hp_breakfast_always_in_table"
                                            && attribute <> "policy_bullet_wrapper")
                            |> Seq.map (fun s ->
                                            {
                                            Class = s.AttributeValue "class"
                                            Text = s.InnerText()
                                            })      
            let filterSpans =spans 
                                |> Seq.where (fun s ->
                                                not (s.Class.Contains "urgency_message_red")
                                                && not (s.Class.Contains "review-score-word--highlighted"))
                                |> Seq.map (fun s -> 
                                                s.Text)

            let rateName = filterSpans |> Seq.tryHead

            if rateName.IsSome
                then rateName.Value
            else ""

let roomsDetails = roomsNodes 
                |> Seq.map(fun roomNode -> 
                    let iteration = Seq.findIndex (fun elem -> elem = roomNode) roomsNodes
                    let dataBlockId = roomNode.AttributeValue "data-block-id"
                    let roomId = dataBlockId.Split [|'_'|] |> Seq.head
                    let occupancy = occupanciesNodes.ElementAt iteration |> getOccupancy                    
                    let roomName = getRoomName roomNode roomId
                    let price = pricesNodes.ElementAt iteration |> getPrice           
                    let rateName = ratesNodes.ElementAt iteration |> getRateName
                    
                    {
                        Iteration = iteration
                        RoomId = roomId
                        Occupancy = occupancy
                        RoomName =roomName
                        RateName =rateName
                        Price = price
                    })

Seq.iter (Printf.printf "%A") roomsDetails


let result = roomsDetails           
                |> Seq.groupBy (fun r -> r.RoomId)                
                |> Seq.map (fun group -> 
                            let roomId = fst group
                            let rooms = snd group
                            let room = rooms
                                        |> Seq.where (fun r -> r.RoomName <> "")
                                        |> Seq.head
                                                                             
                            let rates = rooms
                                        |>  Seq.map (fun r -> 
                                            {
                                                RateName = r.RateName
                                                Price = r.Price
                                                Occupancy = r.Occupancy
                                            })
                                        |> Seq.toArray
                                
                            {
                                RoomId = roomId
                                RoomName = room.RoomName                                    
                                Rate = rates                                    
                            }
                           ) 

Seq.iter (printfn "%A") result

