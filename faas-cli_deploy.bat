	::faas-cli build -f func.yml --no-cache --build-arg platform=linux/arm64
	faas-cli publish -f func.yml --platforms linux/arm64
	faas-cli deploy -f func.yml --gateway http://rp1.local:8080
