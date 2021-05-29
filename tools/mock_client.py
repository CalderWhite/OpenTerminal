import socketio
from datetime import datetime
import random

TICKER = 'AAPL'

sio = socketio.Client()


def get_time_fmtted():
    return datetime.now().strftime("%d/%m/%Y %H:%M:%S %p -05:00")


@sio.on(TICKER)
def on_message(data):
    print("Received data:", data)

@sio.on(TICKER+'-request')
def on_request(data):
    print("Received data request. Sending data..")
    sio.emit(
        'push-trades',
        {
            'ticker': TICKER,
            'trades': [
                {
                    'volume':random.randint(1, 100)*100,
                    'price':random.random()*10,
                    'date':get_time_fmtted()
                }
            ]
        }
    )
    

sio.connect('http://localhost:4201?ticker='+TICKER)

#sio.disconnect()
