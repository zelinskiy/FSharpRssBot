module Network

open Microsoft.FSharp.Control.CommonExtensions   
open System.Net
open System.IO

let private makeRequest (metod:string) (url:string)=        
    async {
        let req = WebRequest.Create(url)
        req.Method = metod |> ignore
        use! resp = req.AsyncGetResponse()
        use stream = resp.GetResponseStream()
        use reader = new StreamReader(stream)
        let html =  reader.ReadToEnd()
        return html
    }

let getAsync = makeRequest "GET"

let postAsync = makeRequest "POST"