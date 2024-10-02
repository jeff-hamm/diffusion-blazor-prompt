docker login https://sjc.vultrcr.com/infinitebutts -u "3ef131a1-17b3-4170-9157-e3d56f40e138" -p "sN3FcwvDkbyFyon8nUrFZZoyd8LYzp4vRfsh"
$Env:ButtsWeb="45.63.49.55"
pushd $PSScriptRoot
try {
    docker compose build "infinite-butts" && docker compose push "infinite-butts"
    scp ./docker-compose.yml root@${Env:ButtsWeb}:.
    scp ./docker-compose.deployed.yml root@${Env:ButtsWeb}:.
    scp ./protobooth.env root@${Env:ButtsWeb}:.
    scp ./nginx.conf root@${Env:ButtsWeb}:.
    ssh root@${Env:ButtsWeb} "docker container stop infinite-butts; docker container rm infinite-butts; docker compose -f ./docker-compose.yml -f docker-compose.deployed.yml down; docker compose -f ./docker-compose.yml -f docker-compose.deployed.yml up --no-build --pull always --force-recreate --remove-orphans"
    # docker pull sjc.vultrcr.com/infinitebutts/butts-prompts; docker container stop infinite-butts;
    # docker container rm infinite-butts; docker run -p 80:8080 -v /mnt/butts/images:/app/wwwroot/butts_images:rw -v /mnt/butts/db:/app/db:rw --restart unless-stopped --name infinite-butts sjc.vultrcr.com/infinitebutts/butts-prompts"
} finally {
    popd
 }