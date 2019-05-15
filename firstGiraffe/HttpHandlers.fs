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
                let result = BookingScraping.scrap(queryParameters.url.ToString())
                let response = ctx.WriteJsonAsync result
                return! json response next ctx
            }