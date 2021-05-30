from datetime import datetime
import logging
import os
import random
import sys

import socketio


class VirtualClient(object):
    def __init__(self, ticker, server_url, logger=logging):
        self.ticker = ticker
        self.server_url = server_url
        self.client = socketio.Client(logger=False)
        self.logger = logger

        self.client.on(self.ticker, self.on_message)
        self.client.on(f'{self.ticker}-request', self.on_request)

    @staticmethod
    def _get_time_str():
        return datetime.now().strftime("%d/%m/%Y %H:%M:%S %p -05:00")

    def run(self):
        self.client.connect(f'{self.server_url}?ticker={self.ticker}')
        self.client.wait()

    def on_message(self, data):
        self.logger.info(data)

    def on_request(self, data):
        self.logger.info("Got ticker request. Sending data...")
        self.client.emit(
            'push-trades',
            {
                'ticker': self.ticker,
                'trades': [
                    {
                        'volume': random.randint(1, 100)*100,
                        'price': random.random()*10,
                        'date': self._get_time_str()
                    }
                ]
            }
        )


def main(args):
    ticker = None
    server_url = None
    logger = logging.Logger('OpenTerminal', level=logging.DEBUG)
    _log_handler = logging.StreamHandler(sys.stdout)
    _log_handler.setFormatter(logging.Formatter('%(name)s:%(levelname)s: %(message)s'))
    logger.addHandler(_log_handler)

    if len(args) >= 2:
        ticker = args[0]
        server_url = args[1]
    else:
        ticker = os.environ.get('OPENTERMINAL_TICKER')
        if ticker is None:
            logger.error('No ticker supplied via environment variables.'\
                         ' you must set OPENTERMINAL_TICKER to the ticker you want.')
            return

        server_url = os.environ.get('OPENTERMINAL_SERVER_URL')
        if server_url is None:
            logger.error('No server URL supplied via environment variables.'\
                         ' you must set OPENTERMINAL_SERVER_URL to your server.')
            return

    logger.info(f'Starting with TICKER:{ticker} and SERVER_URL:{server_url}')
    
    virtual_client = VirtualClient(ticker, server_url, logger=logger)
    virtual_client.run()



if __name__ == '__main__':
    main(sys.argv[1:])
