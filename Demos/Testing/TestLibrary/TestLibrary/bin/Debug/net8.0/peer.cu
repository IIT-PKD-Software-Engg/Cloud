----------------------------967901351434666784066238
Content-Disposition: form-data; name="file"; filename="peer.cu"
Content-Type: application/cu-seeme

// THis change was made for absolute no reason to change this code....i mean its a comment alright....im testing azure function rn

#include <iostream>
#include <time.h>
#include <sys/time.h>
#define USECPSEC 1000000ULL

unsigned long long dtime_usec(unsigned long long start=0){

  timeval tv;
  gettimeofday(&tv, 0);
  return ((tv.tv_sec*USECPSEC)+tv.tv_usec)-start;
}
__global__ void init(char *ptr){
	ptr[0]='A';
	ptr[1]='B';
}
__global__ void  print(char *ptr){
	printf("ptr[0]=%c ptr[1]=%c \n" , ptr[0],	ptr[1]);
}

int main(){
  /*cudaSetDevice(0);
  cudaDeviceEnablePeerAccess(1, 0);
  cudaSetDevice(1);
  cudaDeviceEnablePeerAccess(0, 0);*/
  size_t nbytes = 32768*1024;
  char* src0; // Memory on device 0
  cudaSetDevice(0);
  cudaMalloc(&src0, nbytes);
  init<<<1,1>>>(src0);
  cudaDeviceSynchronize();
  char* dst1; // Memory on device 1
  cudaSetDevice(1);
  cudaMalloc(&dst1, nbytes);
  print<<<1,1>>>(dst1);
  cudaDeviceSynchronize();
  cudaSetDevice(0);
 if( cudaMemcpyPeerAsync(dst1, 1, src0, 0, nbytes)!=cudaSuccess)printf("memcpy error\n");
  cudaDeviceSynchronize();
  cudaSetDevice(1);
  print<<<1,1>>>(dst1);
  cudaDeviceSynchronize();

}

----------------------------967901351434666784066238--
