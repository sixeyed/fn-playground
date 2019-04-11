# Echo API

Simple .NET Core WebApi with an echo endpoint. 

Configured to listen on a Unix socket to work with Fn.

Run in debug mode, and the server is listening on `unix:/tmp/kestrel.sock`.

## Run in Docker

Build (or use my version from Docker Hub):

```
docker image build -t sixeyed/fn-echoapi .
```

Run container with `/tmp' mount for the socket and Fn options:

```
docker container run -d --rm \
 -e FN_FORMAT=http-stream \
 -e FN_LISTENER=unix:/tmp/kestrel.sock \
 -v /tmp/:/tmp/ \
 sixeyed/fn-echoapi
```

> ASP.NET Core creates the socket on startup and the app deletes it on exit

Map the socket to a TCP endpoint:

```
sudo ncat -vlk 8089 -c 'ncat -U /tmp/kestrel.sock'
```

Call the API at `http://localhost:8089/`:

```
curl -X POST \
  http://localhost:8089/call \
   -H 'Content-Type: application/json' \
  -d '"hello"'
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

The issue is to do with the symlink for the socket. It fails when Fn runs [this validation step](https://github.com/fnproject/fn/blob/master/api/agent/agent.go#L938).

Example below:

- Fn runs the container with `FN_LISTENER=/tmp/iofs/lsnr.sock`
- the app creates a socket for internal use at `/tmp/iofs/dotnet-fdk-8dd1ab.sock`
- ASP.NET Core starts listening on that socket
- the app changes the socker permissions to `0666`
- the app creates a symlink from the listener socket to the internal socket:
-  `/tmp/iofs/lsnr.sock -> /tmp/iofs/dotnet-fdk-8dd1ab.sock`

This is the same logic as the [Java FDK](https://github.com/fnproject/fdk-java/blob/master/runtime/src/main/java/com/fnproject/fn/runtime/HTTPStreamCodec.java#L102), as far as I can see.

### Debug logs

```
time="2019-04-11T11:51:52Z" level=debug msg="      Overriding address(es) 'http://+:80'. Binding to endpoints defined in UseKestrel() instead.\n" app_id=01D85WF8M6NG8G008ZJ0000002 container_id=01D863TRRSNG8G008ZJ000000C fn_id=01D85WFS1SNG8G008ZJ0000003 image="sixeyed/fn-echoapi:0.0.19" tag=stderr
time="2019-04-11T11:51:52Z" level=debug msg="fsnotify event" app_id=01D85WF8M6NG8G008ZJ0000002 cpus= event="\"/iofs/iofs947751790/dotnet-fdk-8dd1ab.sock\": CREATE" fn_id=01D85WFS1SNG8G008ZJ0000003 id=01D863TRRSNG8G008ZJ000000C idle_timeout=30 image="sixeyed/fn-echoapi:0.0.19" memory=128
time="2019-04-11T11:51:52Z" level=debug msg="2019-04-11 11:51:52,368 [9] INFO  echoapi.Program [(null)] - Configuring socket, internal: /tmp/iofs/dotnet-fdk-8dd1ab.sock; listener: /tmp/iofs/lsnr.sock\n" app_id=01D85WF8M6NG8G008ZJ0000002 container_id=01D863TRRSNG8G008ZJ000000C fn_id=01D85WFS1SNG8G008ZJ0000003 image="sixeyed/fn-echoapi:0.0.19" tag=stderr
time="2019-04-11T11:51:52Z" level=debug msg="fsnotify event" app_id=01D85WF8M6NG8G008ZJ0000002 cpus= event="\"/iofs/iofs947751790/lsnr.sock\": CREATE" fn_id=01D85WFS1SNG8G008ZJ0000003 id=01D863TRRSNG8G008ZJ000000C idle_timeout=30 image="sixeyed/fn-echoapi:0.0.19" memory=128
time="2019-04-11T11:51:52Z" level=error msg="Failed to check socket destination" app_id=01D85WF8M6NG8G008ZJ0000002 cpus= error="invalid unix socket symlink, symlinks must be relative within the unix socket directory" fn_id=01D85WFS1SNG8G008ZJ0000003 id=01D863TRRSNG8G008ZJ000000C idle_timeout=30 image="sixeyed/fn-echoapi:0.0.19" memory=128
time="2019-04-11T11:51:52Z" level=error msg="api error" action="server.handleFnInvokeCall)-fm" code=502 error="container failed to initialize, please ensure you are using the latest fdk / format and check the logs" fn_id=01D85WFS1SNG8G008ZJ0000003
```

