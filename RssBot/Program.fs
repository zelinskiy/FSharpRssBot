open System.Globalization
open System.Xml
open System.Text
open System.Net
open System

type RssPost = { title: string; link: string; date: DateTime }

let winToUtf (s:string) = 
    let win1251 = Encoding.GetEncoding("windows-1251")
    let utf = new UTF8Encoding()
    let encodedBytes = win1251.GetBytes s
    utf.GetString encodedBytes    

let flip4 f a b c d = f(d, c, b, a)

let parseDate = (flip4 (DateTime.ParseExact:string*string[]*CultureInfo*DateTimeStyles->DateTime)) 
                        DateTimeStyles.None
                        CultureInfo.InvariantCulture
                        [|"ddd, dd MMM yyyy HH:mm:ss GMT"; "ddd, dd MMM yyyy HH:mm:ss zzz"|]

let getRssPosts (host:string) = 
    let  wc = new WebClient()
    let doc = new XmlDocument()
    host 
        |> wc.DownloadString 
        |> winToUtf 
        |> doc.LoadXml
    (doc.SelectNodes "/rss/channel/item"
        |> Seq.cast<XmlNode>
        |> Seq.map (fun node -> 
            { 
                title = (node.SelectSingleNode "title").InnerText;
                link = (node.SelectSingleNode "link").InnerText;
                date = parseDate (node.SelectSingleNode "pubDate").InnerText
            }))



[<EntryPoint>]
let main argv = 
    ["https://news.google.com/?output=rss"; "http://nure.ua/category/all_news/feed/"]
        |> Seq.map getRssPosts
        |> Seq.concat
        |> Seq.sortBy (fun p -> p.date)
        |> Seq.iter (fun (p:RssPost) -> Console.WriteLine (p.date.ToString "dd/MM/yyyy"))    
    Console.ReadLine() |> ignore
    0
