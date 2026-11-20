from http.server import BaseHTTPRequestHandler, HTTPServer
from urllib.parse import unquote, urlsplit
from xml.etree.ElementTree import Element, SubElement, tostring
import os

objects = {}
bucket = "/fixture-inventory"
token = "AWS " + os.environ["FIXTURE_ACCESS_KEY"] + ":"

class Handler(BaseHTTPRequestHandler):
    def log_message(self, *args):
        pass

    def reply(self, code, contents=b"", kind="text/plain"):
        self.send_response(code)
        self.send_header("Content-Length", str(len(contents)))
        self.send_header("Content-Type", kind)
        self.send_header("ETag", '"demo"')
        self.send_header("Last-Modified", "Thu, 01 Jan 2026 00:00:00 GMT")
        self.end_headers()
        self.wfile.write(contents)

    def dispatch(self):
        path = unquote(urlsplit(self.path).path)
        if path == "/health":
            return self.reply(200, b"ready")
        if not self.headers.get("Authorization", "").startswith(token):
            return self.reply(403)
        print(self.command, "list" if path.rstrip("/") == bucket else "object", flush=True)
        if path.rstrip("/") == bucket and self.command == "GET":
            root = Element("ListBucketResult", xmlns="http://s3.amazonaws.com/doc/2006-03-01/")
            for name, value in (("Name", "fixture-inventory"), ("Prefix", "demo/"), ("Marker", ""),
                                ("MaxKeys", "1000"), ("IsTruncated", "false")):
                SubElement(root, name).text = value
            for entry, payload in objects.items():
                element = SubElement(root, "Contents")
                for name, value in (("Key", entry.removeprefix(bucket + "/")),
                                    ("LastModified", "2026-01-01T00:00:00Z"), ("ETag", '"demo"'),
                                    ("Size", str(len(payload))),
                                    ("StorageClass", "STANDARD")):
                    SubElement(element, name).text = value
            return self.reply(200, tostring(root), "application/xml")
        if not path.startswith(bucket + "/demo/"):
            return self.reply(404)
        if self.command == "PUT":
            length = int(self.headers.get("Content-Length", "0"))
            if not 0 < length <= 4096:
                return self.reply(400)
            objects[path] = self.rfile.read(length)
            return self.reply(200)
        if self.command == "DELETE":
            objects.pop(path, None)
            return self.reply(204)
        return self.reply(200, objects[path]) if path in objects else self.reply(404)

    do_GET = do_PUT = do_DELETE = dispatch

HTTPServer(("0.0.0.0", 8090), Handler).serve_forever()
