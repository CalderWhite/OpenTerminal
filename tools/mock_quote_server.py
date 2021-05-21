from flask import Flask, request
from datetime import datetime
import random

app = Flask(__name__)

base = {
    "aapl" : 125,
    "msft" : 248,
    "amzn" : 3222,
    "tsla" : 589
}

def getBase(sym):
    global base
    
    if sym in base.keys():
        return base[sym]
    else:
        base[sym] = random.randint(10, 4000)
        return base[sym]

def setBase(sym, n):
    base[sym] = n

def formatDollar(flt):
    return format(flt, ".2f")

def genSym(symbol, limit, time):
    data = {}
    data["symbol"] = symbol
    data["totalRecords"] = 100
    data["offset"] = 0
    data["limit"] = limit
    data["headers"] = {
        "nlsTime" : "NLS Time (ET)",
        "nlsPrice" : "NLS Price",
        "nlsShareVolume" : "NLS Share Volume"
    }

    pf = getBase(symbol) * (1 + (random.random()-0.5)/25)
    setBase(symbol, pf)
    price = formatDollar(pf)

    data["rows"] = [
        {
            "nlsTime":time.strftime("%H:%M:%S"),
            "nlsPrice":"$ "+price,
            "nlsShareVolume": str(random.randint(10, 10000))
        } for _ in range(limit)
    ]

    data["topTable"] = {
        "headers":{
            "nlsVolume" : "NLS Volume",
            "previousClose" : "Previous Close",
            "todayHighLow" : "Today's High / Low",
            "fiftyTwoWeekHighLow" : "52 Week High / Low"
         },
         "rows": [
             {
                 "nlsVolume" : "{:,}".format(random.randint(200000, 100000000)),
                 "previousClose" : "$"+formatDollar(getBase(symbol) * (1 + (random.random()-0.5)/25)),
                 "todayHighLow" : "$"+formatDollar(pf*1.01)+"/$"+formatDollar(pf*0.99),
                 "fiftyTwoWeekHighLow" : "$"+formatDollar(pf*1.05)+"/$"+formatDollar(pf*0.95)
             }
         ]
    }

    return data


@app.route("/api/quote/<symbol>/realtime-trades")
def hello_world(symbol):
    print(datetime.now(), "REQUEST")
    print("symbol = ", symbol)
    time = datetime.now()
    print("request time = ", time)
    limit = int(request.args.get("limit"))
    print("limit =", limit)
    print("fromTime =", request.args.get("fromTime"))
    
    return {
        "data" : genSym(symbol.lower(), limit if limit else 20, time),
        "message" : None,
        "status" : {
            "rCode" : 200,
            "bCodeMessage" : None,
            "developerMessage" : None
        }
    }
    

if __name__ == '__main__':
    app.run()
