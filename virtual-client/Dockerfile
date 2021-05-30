FROM python:3.9.5-slim

WORKDIR /usr/src/OpenTerminal/virtual-client

COPY requirements.txt ./
RUN pip install --no-cache-dir -r requirements.txt

COPY . .

CMD [ "python", "./virtual_client.py"]
