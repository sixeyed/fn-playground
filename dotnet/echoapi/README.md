# Echo API

Simple .NET Core WebApi with an echo endpoint. 

Configured to listen on a Unix socket to work with Fn.

Run in debug mode, and the server is listening on `unix:/tmp/kestrel.sock`.

## Run in Docker

Mount to run in a container:

```
docker container run -it --rm \
 -e FN_FORMAT=http-stream \
 -e FN_LISTENER=unix:/tmp/kestrel.sock \
 -v /tmp/:/tmp/ \
 sixeyed/fn-echoapi
```

> ASP.NET Core creates the socket on startup

Map the socket to a TCP endpoint:

```
sudo ncat -vlk 8089 -c 'ncat -U /tmp/kestrel.sock'
```

Call the API at `http://localhost:8089/`:

```
curl http://localhost:8089/call/echo-me
```

## Run with Fn

Start Fn & create an app:

```
fn start --log-level=debug
```

```
fn create app dotnet
```

Deploy the function:

```
fn deploy --app dotnet --local --verbose
```

Invoke the function:

```
DEBUG=1 fn invoke dotnet fn-echoapi
```

> Fails right now

### Debug logs

```
time="2019-03-26T20:46:10Z" level=debug msg="Hot function launcher starting" action="server.handleFnInvokeCall)-fm" app_id=01D6XTRV79NG8G00RZJ0000001 fn_id=01D6XTT248NG8G00RZJ0000002 id=01D6XW1NANNG8G008ZJ0000001 launcher_timeout=1h0m0s
time="2019-03-26T20:46:10Z" level=debug msg="setting tmpfs" app_id=01D6XTRV79NG8G00RZJ0000001 call_id=01D6XW1NAPNG8G008ZJ0000002 cpus= fn_id=01D6XTT248NG8G00RZJ0000002 id=01D6XW1NAPNG8G008ZJ0000002 idle_timeout=30 image="sixeyed/fn-echoapi:0.0.5" memory=128 options= stack=CreateCookie target=/tmp
time="2019-03-26T20:46:10Z" level=debug msg="setting hostname" app_id=01D6XTRV79NG8G00RZJ0000001 call_id=01D6XW1NAPNG8G008ZJ0000002 cpus= fn_id=01D6XTT248NG8G00RZJ0000002 hostname=a8000252b5a3 id=01D6XW1NAPNG8G008ZJ0000002 idle_timeout=30 image="sixeyed/fn-echoapi:0.0.5" memory=128 stack=CreateCookie
time="2019-03-26T20:46:10Z" level=debug msg="docker inspect image" app_id=01D6XTRV79NG8G00RZJ0000001 call_id=01D6XW1NAPNG8G008ZJ0000002 cpus= fn_id=01D6XTT248NG8G00RZJ0000002 id=01D6XW1NAPNG8G008ZJ0000002 idle_timeout=30 image="sixeyed/fn-echoapi:0.0.5" memory=128 stack=ValidateImage
time="2019-03-26T20:46:10Z" level=debug msg="docker create container" app_id=01D6XTRV79NG8G00RZJ0000001 call_id=01D6XW1NAPNG8G008ZJ0000002 cpus= fn_id=01D6XTT248NG8G00RZJ0000002 id=01D6XW1NAPNG8G008ZJ0000002 idle_timeout=30 image="sixeyed/fn-echoapi:0.0.5" memory=128 stack=CreateContainer
time="2019-03-26T20:46:11Z" level=debug msg="2019-03-26 20:46:11,711 [1] INFO  echoapi.Program [(null)] - Echo Api starting...\n" app_id=01D6XTRV79NG8G00RZJ0000001 container_id=01D6XW1NAPNG8G008ZJ0000002 fn_id=01D6XTT248NG8G00RZJ0000002 image="sixeyed/fn-echoapi:0.0.5" tag=stderr
time="2019-03-26T20:46:11Z" level=debug msg="2019-03-26 20:46:11,742 [1] INFO  echoapi.Program [(null)] - Listening on unix socket: /tmp/iofs/lsnr.sock\n" app_id=01D6XTRV79NG8G00RZJ0000001 container_id=01D6XW1NAPNG8G008ZJ0000002 fn_id=01D6XTT248NG8G00RZJ0000002 image="sixeyed/fn-echoapi:0.0.5" tag=stderr
time="2019-03-26T20:46:12Z" level=debug msg=": Microsoft.AspNetCore.DataProtection.Repositories.EphemeralXmlRepository[50]\n" app_id=01D6XTRV79NG8G00RZJ0000001 container_id=01D6XW1NAPNG8G008ZJ0000002 fn_id=01D6XTT248NG8G00RZJ0000002 image="sixeyed/fn-echoapi:0.0.5" tag=stderr
time="2019-03-26T20:46:12Z" level=debug msg="      Using an in-memory repository. Keys will not be persisted to storage.\n" app_id=01D6XTRV79NG8G00RZJ0000001 container_id=01D6XW1NAPNG8G008ZJ0000002 fn_id=01D6XTT248NG8G00RZJ0000002 image="sixeyed/fn-echoapi:0.0.5" tag=stderr
time="2019-03-26T20:46:12Z" level=debug msg="\x1b[40m\x1b[1m\x1b[33mwarn\x1b[39m\x1b[22m\x1b[49m: Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager[59]\n" app_id=01D6XTRV79NG8G00RZJ0000001 container_id=01D6XW1NAPNG8G008ZJ0000002 fn_id=01D6XTT248NG8G00RZJ0000002 image="sixeyed/fn-echoapi:0.0.5" tag=stderr
time="2019-03-26T20:46:12Z" level=debug msg="      Neither user profile nor HKLM registry available. Using an ephemeral key repository. Protected data will be unavailable when application exits.\n" app_id=01D6XTRV79NG8G00RZJ0000001 container_id=01D6XW1NAPNG8G008ZJ0000002 fn_id=01D6XTT248NG8G00RZJ0000002 image="sixeyed/fn-echoapi:0.0.5" tag=stderr
time="2019-03-26T20:46:12Z" level=debug msg="\x1b[40m\x1b[1m\x1b[33mwarn\x1b[39m\x1b[22m\x1b[49m: Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager[35]\n" app_id=01D6XTRV79NG8G00RZJ0000001 container_id=01D6XW1NAPNG8G008ZJ0000002 fn_id=01D6XTT248NG8G00RZJ0000002 image="sixeyed/fn-echoapi:0.0.5" tag=stderr
time="2019-03-26T20:46:12Z" level=debug msg="      No XML encryptor configured. Key {dcdea679-f457-41d4-9a0e-75e05f230a85} may be persisted to storage in unencrypted form.\n" app_id=01D6XTRV79NG8G00RZJ0000001 container_id=01D6XW1NAPNG8G008ZJ0000002 fn_id=01D6XTT248NG8G00RZJ0000002 image="sixeyed/fn-echoapi:0.0.5" tag=stderr
time="2019-03-26T20:46:12Z" level=debug msg="\x1b[40m\x1b[1m\x1b[33mwarn\x1b[39m\x1b[22m\x1b[49m: Microsoft.AspNetCore.Server.Kestrel[0]\n" app_id=01D6XTRV79NG8G00RZJ0000001 container_id=01D6XW1NAPNG8G008ZJ0000002 fn_id=01D6XTT248NG8G00RZJ0000002 image="sixeyed/fn-echoapi:0.0.5" tag=stderr
time="2019-03-26T20:46:12Z" level=debug msg="      Overriding address(es) 'http://+:80'. Binding to endpoints defined in UseKestrel() instead.\n" app_id=01D6XTRV79NG8G00RZJ0000001 container_id=01D6XW1NAPNG8G008ZJ0000002 fn_id=01D6XTT248NG8G00RZJ0000002 image="sixeyed/fn-echoapi:0.0.5" tag=stderr
time="2019-03-26T20:46:12Z" level=debug msg="fsnotify event" app_id=01D6XTRV79NG8G00RZJ0000001 cpus= event="\"/iofs/iofs049060159/lsnr.sock\": CREATE" fn_id=01D6XTT248NG8G00RZJ0000002 id=01D6XW1NAPNG8G008ZJ0000002 idle_timeout=30 image="sixeyed/fn-echoapi:0.0.5" memory=128
time="2019-03-26T20:46:12Z" level=info msg="starting call" action="server.handleFnInvokeCall)-fm" app_id=01D6XTRV79NG8G00RZJ0000001 container_id=01D6XW1NAPNG8G008ZJ0000002 fn_id=01D6XTT248NG8G00RZJ0000002 id=01D6XW1NANNG8G008ZJ0000001
time="2019-03-26T20:46:12Z" level=info msg="hot function terminating" app_id=01D6XTRV79NG8G00RZJ0000001 cpus= error="Post http://localhost/call: dial unix /iofs/iofs049060159/lsnr.sock: connect: connection refused" fn_id=01D6XTT248NG8G00RZJ0000002 id=01D6XW1NAPNG8G008ZJ0000002 idle_timeout=30 image="sixeyed/fn-echoapi:0.0.5" memory=128
time="2019-03-26T20:46:12Z" level=error msg="api error" action="server.handleFnInvokeCall)-fm" code=502 error="error receiving function response" fn_id=01D6XTT248NG8G00RZJ0000002
```