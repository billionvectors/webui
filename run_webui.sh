set -e

if [ "$1" == "--clean" ]; then
    docker rmi asimplevectors_webui --force || true
fi

mkdir -p data
mkdir -p keys
sudo chmod -R 777 data
sudo chmod -R 777 keys

docker build -t asimplevectors_webui . || exit 1
docker run -v $(pwd)/data:/data -v $(pwd)/keys:/keys -e ASPNETCORE_ENVIRONMENT=Development -p 21080:8080 asimplevectors_webui
