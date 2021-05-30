# Virtual Client

These clients run on minimal instances in the cloud to increase the interval at which you recieve
trade updates.

# Running with Docker

**NOTE:** This demo is broken for local demos.

Note: The `--network=host` option is not required for ips and hostnames outside of your local network. You
also **MUST** use your actual ip and not `localhost` or `127.0.0.1` as the docker container will route that to itself.

```
docker run -e "OPENTERMINAL_TICKER=AAPL" -e "OPENTERMINAL_SERVER_URL=http://192.168.2.16:4201" --network=host calderwhite/openterminal-virtualclient:latest
```

# Running Locally
You must set both the `OPENTERMINAL_TICKER` and `OPENTERMINAL_SERVER_URL`. You may either do this via
argv (`python3 virtual_client [OPENTERMINAL_TICKER] [OPENTERMINAL_SERVER_URL]`),or through environment
variables. Here is an example for AAPL and localhost:

```bash
export OPENTERMINAL_TICKER=AAPL
export OPENTERMINAL_SERVER_URL="http://localhost:4201"

python virtual_client.py
```

# Building Docker
`docker build . --tag calderwhite/openterminal-virtualclient:latest`
