FROM nvcr.io/nvidia/tensorrt:22.12-py3
WORKDIR /home/user
RUN useradd -m -u 1000 user

WORKDIR /home/user/app
RUN --mount=target=/root/packages.txt,source=packages.txt apt-get update && \
	xargs -r -a /root/packages.txt apt-get install -y 	&& \
	rm -rf /var/lib/apt/lists/*
ENV DEBIAN_FRONTEND=noninteractive 
RUN apt-get update && \
	apt-get install -y 	git make build-essential libssl-dev zlib1g-dev 	libbz2-dev libreadline-dev libsqlite3-dev wget curl llvm && \
    apt-get install -y libncursesw5-dev xz-utils tk-dev libxml2-dev libxmlsec1-dev libffi-dev liblzma-dev git-lfs ffmpeg libsm6 libxext6 cmake libgl1-mesa-glx 	&& \
	rm -rf /var/lib/apt/lists/* 	&& \
	git lfs install
WORKDIR /home/user
ENV HOME  /home/user
ENV PYENV_ROOT $HOME/.pyenv
ENV PATH $PYENV_ROOT/shims:$PYENV_ROOT/bin:$PATH
RUN git clone https://github.com/yyuu/pyenv.git .pyenv && \
    pyenv install 3.10 && pyenv global 3.10 && pyenv rehash && pip install --no-cache-dir --upgrade pip==22.3.1 setuptools wheel && pip install --no-cache-dir 	datasets "huggingface-hub>=0.12.1" "protobuf<4" "click<8.1" "pydantic~=1.0"
WORKDIR /home/user/app
#--mount=target=pre-requirements.txt,source=pre-requirements.txt 
#RUN pip install --no-cache-dir -r pre-requirements.txt
#
RUN  --mount=target=pre-requirements.txt,source=pre-requirements.txt pip install --no-cache-dir -r pre-requirements.txt
RUN --mount=target=requirements.txt,source=requirements.txt 	pip install --no-cache-dir -r requirements.txt
#COPY --link --chown=1000 --from=lfs /app /home/user/app
#COPY --link --chown=1000 ./ /home/user/app
RUN pip freeze > /tmp/freeze.txt
#COPY --from=pipfreeze --link --chown=1000 /tmp/freeze.txt .
ENV HOME=/home/user \
    PATH=/home/user/.local/bin:$PATH \
	TF_ENABLE_ONEDNN_OPTS=0 \
	TF_CPP_MIN_LOG_LEVEL=1
COPY . .
EXPOSE 7860
CMD ["python", "app.py"]
