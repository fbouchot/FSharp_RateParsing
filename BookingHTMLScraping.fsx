#r "packages/FSharp.Data.3.0.0/lib/net45/FSharp.Data.dll"
#r "System.Net"
#load "Core.fs"
#load "MealPlanParsing.fs"

open FSharp.Data
open System.Text.RegularExpressions
open FSharp.Core
open Availpro.RateCategorisation.Parsing
open System.Web.UI.HtmlControls
open System.Linq
open System.Linq

// Path functions
let root = __SOURCE_DIRECTORY__
let (</>) x y = System.IO.Path.Combine(x, y)

let BookingUrl = "https://www.booking.com/hotel/es/vincci-gala.en-gb.html?checkin=2019-05-29&checkout=2019-05-30&group_adults=2&selected_currency=EUR&sb_price_type=total&type=total&do_availability_check=1"

let HtmlResponse = 
    Http.RequestString(
        BookingUrl, 
        httpMethod="GET", 
        headers=[("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36")])

   
printfn "%s" HtmlResponse

let samplePath = root </> "Html/demo_20190513.html"
System.IO.File.WriteAllText(samplePath, HtmlResponse)

let html = System.IO.File.ReadAllText(samplePath)

let doc = HtmlDocument.Load(samplePath)

let table = doc.Descendants("table").Where(fun x -> 
                x.AttributeValue("class").StartsWith("hprt-table")).FirstOrDefault()

let prices = doc.Descendants("td").Where(fun td -> 
                td.AttributeValue("class").Contains("hprt-table-cell-price") 
                && td.AttributeValue("class").Contains("wholesalers") <> true)                
                |> Seq.map (fun cell -> 
                    cell.Descendants("div").FirstOrDefault(fun c -> c.AttributeValue("class").Contains("hprt-price-price")).InnerText())

                    
let occupancies = doc.Descendants("td").Where(fun td -> 
                td.AttributeValue("class").Contains("hprt-table-cell-occupancy") 
                && td.AttributeValue("class").Contains("wholesalers") <> true)                
                |> Seq.map (fun cell -> 
                    cell.Descendants("i").Count(fun c -> c.AttributeValue("class").Contains("bicon-occupancy"))


let tableString = table.ToString()

prices |> Seq.map (fun price -> printf "%s" price)

let priceText = prices.FirstOrDefault()

printf "%A" prices