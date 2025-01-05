set -e

if [ "$1" == "--clean" ]; then
    docker rmi asimplevectors_webui --force || true
fi

docker build -t asimplevectors_webui . || exit 1
docker run -v keys:/keys -e ASPNETCORE_ENVIRONMENT=Development -p 21080:8080 asimplevectors_webui
