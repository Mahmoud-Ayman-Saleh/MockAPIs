import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  stages: [
    { duration: '5s', target: 50 },  // ramp up to 50 users
    { duration: '15s', target: 50 }, // stay at 50 users for 15s
    { duration: '5s', target: 0 },   // ramp down to 0 users
  ],
};

export default function () {
  const url = 'http://localhost:5167/testtoken123/api/v1/test-resource?page=1&limit=10';
  const params = {
    headers: {
      'Accept': 'application/json',
    },
  };

  const res = http.get(url, params);
  
  check(res, {
    'is status 200': (r) => r.status === 200,
  });
  
  sleep(1);
}
