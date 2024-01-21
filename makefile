build:
	dotnet publish ./webiconsetter.csproj -c Release -o dist -r linux-x64 --self-contained true

dist: build
	dotnet publish ./webiconsetter.csproj -c Release -o dist -r linux-x64 --self-contained true
	rm -rf app
	mkdir app
	mkdir app/DEBIAN
	mkdir app/usr
	mkdir app/usr/bin
	mkdir app/usr/share
	mkdir app/usr/share/webiconsetter
	mkdir app/usr/share/applications
	cp ./assets/webiconsetter.desktop app/usr/share/applications/webiconsetter.desktop
	cp ./assets/logo.png app/usr/share/webiconsetter/logo.png
	cp ./assets/black196.png app/usr/share/webiconsetter/black196.png

	cp ./dist/webiconsetter app/usr/bin/webiconsetter
	chmod +x app/usr/bin/webiconsetter
	cp ./assets/control app/DEBIAN/control
	cp ./assets/postinst app/DEBIAN/postinst
	chmod +x app/DEBIAN/postinst

	dpkg-deb --build app
	mv app.deb pioneos-webiconsetter_1.0-1_amd64.deb
clean:
	rm -rf app
	rm -rf dist
	rm -rf bin
	rm -rf obj
	rm -rf app
	rm -rf dist
	rm -rf bin
	rm -rf obj