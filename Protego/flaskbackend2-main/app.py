from flask import Flask, jsonify, request
from sklearn.preprocessing import StandardScaler
import joblib
import numpy as np
from urllib.parse import urlparse

app = Flask(__name__)

voting_model = joblib.load('ensemble_model.joblib')
scaler = joblib.load('scaler2.joblib')

def extract_features_for_url(url):
    parsed_url = urlparse(url)
    return [
        len(url),
        url.count('.'),
        url.count('/'),
        len(parsed_url.path.split('/')),
        len(parsed_url.query.split('&')),
        1 if parsed_url.scheme == 'https' else 0,
        len(parsed_url.netloc),
        url.count('-'),
        url.count('@'),
        url.count('?'),
        url.count('='),
        1 if "client" in url.lower() else 0,  
        1 if "admin" in url.lower() else 0, 
        1 if parsed_url.port else 0,          
        1 if parsed_url.hostname else 0,     
        1 if "login" in url.lower() else 0,  
        url.count('%20'),
        len(parsed_url.fragment.split('&')),
        
    ]

@app.route('/process_url', methods=['POST'])
def process_url():
    url = request.json.get('url')
    if url:
        features = extract_features_for_url(url)
        prediction = voting_model.predict([features])[0]
        
        label = 'Safe Website' if prediction == 1 else 'Potentially Dangerous'
        return jsonify({'prediction': label})
    else:
        return jsonify({'error': 'No URL provided'})

if __name__ == '__main__':
    app.run(debug=True)
