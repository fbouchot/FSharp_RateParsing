namespace firstGiraffe

module HttpHandlers =
    
    open FSharp.Data
    open Availpro.RateCategorisation.Parsing
    open Microsoft.AspNetCore.Http
    open FSharp.Control.Tasks.V2.ContextInsensitive
    open Giraffe
    open firstGiraffe.Models
    open Newtonsoft.Json
    

    let handleGetHello =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let response = {
                    Text = "Hello world, from Giraffe!"
                }
                return! json response next ctx
            }    
    
    let handleScrap =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let queryParameters = ctx.BindQueryString<QueryParameters>()   
                let url = queryParameters.url + "?checkin=" + queryParameters.checkin + "&checkout="+ queryParameters.checkout+ "&group_adults=" + queryParameters.adult + "&selected_currency=" + queryParameters.currency + "&sb_price_type=total&type=total&do_availability_check=1"
                let result = BookingScraping.scrap url
                let response = ctx.WriteJsonAsync result
                return! json response next ctx
            }