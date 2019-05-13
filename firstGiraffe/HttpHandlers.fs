namespace firstGiraffe

module HttpHandlers =

    open Microsoft.AspNetCore.Http
    open FSharp.Control.Tasks.V2.ContextInsensitive
    open Giraffe
    open firstGiraffe.Models
    

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
                let response = {
                    Text = queryParameters.url.ToString()
                }
                return! json response next ctx
            }