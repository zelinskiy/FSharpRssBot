module Rss

open Network

open System.Globalization
open System.Xml
open System.Text
open System.Net
open System.IO
open System.Linq
open System.Collections.Generic
open System

type RssPost = { title: string; link: string; date: DateTime }

let private winToUtf (s:string) = 
    let win1251 = Encoding.GetEncoding("windows-1251")
    let utf = new UTF8Encoding()
    let encodedBytes = win1251.GetBytes s
    utf.GetString encodedBytes    

let private flip4 f a b c d = f(d, c, b, a)

let parseDate = (flip4 (DateTime.ParseExact:string*string[]*CultureInfo*DateTimeStyles->DateTime)) 
                        DateTimeStyles.None
                        CultureInfo.InvariantCulture
                        [|"ddd, dd MMM yyyy HH:mm:ss GMT"; "ddd, dd MMM yyyy HH:mm:ss zzz"|]

let private parseNode (node:XmlNode) = 
    { 
        title = (node.SelectSingleNode "title").InnerText;
        link = (node.SelectSingleNode "link").InnerText;
        date = parseDate (node.SelectSingleNode "pubDate").InnerText
    }

let public getRssPosts (host:string) = 
    let wc = new WebClient()
    let doc = new XmlDocument()
    host 
        |> Network.getAsync
        |> Async.RunSynchronously
        //|> winToUtf 
        |> doc.LoadXml
    doc.SelectNodes "/rss/channel/item"
        |> Seq.cast<XmlNode>
        |> Seq.map parseNode

let public getManyRssPosts: (list<string> -> list<RssPost>) = 
    Seq.map getRssPosts 
    >> Seq.concat 
    >> Seq.toList