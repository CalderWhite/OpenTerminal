"""Generates C# code for testing the BaseConnector."""


x = """10:59:58	$ 126.3602	100
10:59:58	$ 126.36	100
10:59:58	$ 126.36	100
10:59:58	$ 126.3676	300
10:59:57	$ 126.36	100
10:59:57	$ 126.36	100
10:59:57	$ 126.36	172
10:59:57	$ 126.36	147
10:59:57	$ 126.36	153
10:59:56	$ 126.3501	3,000
10:59:56	$ 126.355	100
10:59:55	$ 126.35	100"""



for line in x.split("\n"):
    time, price, shares = line.replace("$","").split()
    print('trades.Add(new Trade("$%s", "%s", "%s"));' % (price, shares, time))
